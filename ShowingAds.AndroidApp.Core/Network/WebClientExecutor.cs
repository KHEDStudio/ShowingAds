using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.Interfaces;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShowingAds.AndroidApp.Core.Network
{
    public class WebClientExecutor<T> : IExecutor<T> where T : EventArgs
    {
        private Thread _consumerThread;
        private readonly object _syncCurrent = new object();

        private IWebClientCommand _current;
        private BlockingCollection<IWebClientCommand> _queue;

        private CancellationTokenSource _cancellationToken;

        public event Action<T> CommandExecuted;
        public event Action<ProgressChangedEventArgs> ProgressChanged;

        public WebClientExecutor()
        {
            _cancellationToken = new CancellationTokenSource();
            _queue = new BlockingCollection<IWebClientCommand>();
            _consumerThread = new Thread(() => RunConsumer());
            _consumerThread.Start();
        }

        public void AddCommandToQueue(IWebClientCommand command) => _queue.Add(command);

        private void RunConsumer()
        {
            while (_cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    var middle = _queue.Take(_cancellationToken.Token);
                    lock (_syncCurrent)
                    {
                        _current = middle;
                        _current.Completed += CurrentCompleted;
                        _current.ProgressChanged += CurrentProgressChanged;
                    }
                    ProgressChanged?.Invoke(new ProgressChangedEventArgs(_queue.Count, default));
                    _current.Execute();
                }
                catch (Exception ex)
                {
                    if (_cancellationToken.IsCancellationRequested == false)
                    {
                        _queue.Add(_current);
                        ServerLog.Error("WebClientExecutor", ex.Message);
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                }
                finally
                {
                    lock (_syncCurrent)
                    {
                        if (_current != null)
                        {
                            _current.Completed -= CurrentCompleted;
                            _current.ProgressChanged -= CurrentProgressChanged;
                            _current = null;
                        }
                    }
                }
            }
        }

        private void CurrentCompleted(EventArgs obj) => CommandExecuted?.Invoke((T)obj);

        private void CurrentProgressChanged(ProgressChangedEventArgs obj) => ProgressChanged?.Invoke(obj);

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
            _cancellationToken.Cancel();
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
