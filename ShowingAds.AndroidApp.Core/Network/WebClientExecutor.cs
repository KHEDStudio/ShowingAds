using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.Interfaces;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShowingAds.AndroidApp.Core.Network
{
    public class WebClientExecutor<T> : IExecutor<T> where T : EventArgs
    {
        private readonly object _syncCurrent = new object();

        private IWebClientCommand _current;
        private BlockingCollection<IWebClientCommand> _queue;

        public event Action<T> CommandExecuted;

        public WebClientExecutor()
        {
            _queue = new BlockingCollection<IWebClientCommand>();
            Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
        }

        public void AddCommandToQueue(IWebClientCommand command) => _queue.Add(command);

        private async Task Run()
        {
            while (_queue.IsAddingCompleted == false)
            {
                try
                {
                    var middle = _queue.Take();
                    lock (_syncCurrent)
                    {
                        _current = middle;
                        _current.Completed += CurrentCompleted;
                        _current.ProgressChanged += CurrentProgressChanged;
                    }
                    await _current.Execute();
                }
                catch (Exception ex)
                {
                    if (_queue.IsAddingCompleted == false)
                    {
                        _queue.Add(_current);
                        await ServerLog.Error("WebClientExecutor", ex.Message);
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
                finally
                {
                    lock (_syncCurrent)
                    {
                        _current.Completed -= CurrentCompleted;
                        _current.ProgressChanged -= CurrentProgressChanged;
                        _current = null;
                    }
                }
            }
        }

        private void CurrentCompleted(EventArgs obj) => CommandExecuted?.Invoke((T)obj);

        private void CurrentProgressChanged(System.Net.DownloadProgressChangedEventArgs obj) => 
            ServerLog.Debug("ProgressChanged", obj.ProgressPercentage.ToString());

        public void Filter(BaseFilter filter)
        {
            var filteredCommands = new List<IWebClientCommand>();
            while (_queue.TryTake(out var command))
                if (command.IsValid(filter))
                    filteredCommands.Add(command);
            lock (_syncCurrent)
                if (_current != null && _current.IsValid(filter) == false)
                    _current.Undo();
            foreach (var command in filteredCommands)
                _queue.Add(command);
        }

        public void Dispose()
        {
            _queue.CompleteAdding();
            lock (_syncCurrent)
                _current?.Undo();
        }

        public void Accept(BaseVisitor visitor)
        {
            var commands = new List<IWebClientCommand>();
            while (_queue.TryTake(out var command))
            {
                command.Accept(visitor);
                commands.Add(command);
            }
            lock (_syncCurrent)
                if (_current != null)
                    _current.Accept(visitor);
            foreach (var command in commands)
                _queue.Add(command);
        }
    }
}
