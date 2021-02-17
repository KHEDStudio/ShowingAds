using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters
{
    public class LogotypeFilter : BaseFilter
    {
        /* bool is flag shows validation logo */
        private (bool, Guid) _leftLogotype;
        private (bool, Guid) _rightLogotype;

        public LogotypeFilter(Guid leftLogotype, Guid rightLogotype)
        {
            _leftLogotype = (false, leftLogotype);
            _rightLogotype = (false, rightLogotype);
        }

        public override bool FilterClientInterval(ClientInterval interval) => throw new ArgumentException("Cannot filter client interval");

        public override bool FilterClientOrder(ClientOrder order) => throw new ArgumentException("Cannot filter client order");

        public override bool FilterLogoCommand(LogoDownloadCommand command)
        {
            if (command.IsLeft)
                return command.Id == _leftLogotype.Item2;
            if (command.IsLeft == false)
                return command.Id == _rightLogotype.Item2;
            return false;
        }

        public override bool FilterLogotype(Logotype logotype)
        {
            if (logotype.IsLeft)
            {
                if (_leftLogotype.Item2 == logotype.Id)
                {
                    _leftLogotype = (true, logotype.Id);
                    return true;
                }
            }
            if (logotype.IsLeft == false)
            {
                if (_rightLogotype.Item2 == logotype.Id)
                {
                    _rightLogotype = (true, logotype.Id);
                    return true;
                }
            }
            return false;
        }

        public override bool FilterVideo(Video video) => throw new ArgumentException("Cannot filter video");

        public override bool FilterVideoCommand(VideoDownloadCommand command) => throw new ArgumentException("Cannot filter video download command");

        public override IEnumerable<BaseDownloadCommand> GetDownloadCommands()
        {
            if (_leftLogotype.Item1 == false && _leftLogotype.Item2 != Guid.Empty)
                yield return new LogoDownloadCommand(Settings.GetLogoDownloadUri(_leftLogotype.Item2),
                    Settings.GetLogotypeFilesPath(_leftLogotype.Item2), _leftLogotype.Item2, true);
            if (_rightLogotype.Item1 == false && _rightLogotype.Item2 != Guid.Empty)
                yield return new LogoDownloadCommand(Settings.GetLogoDownloadUri(_rightLogotype.Item2),
                    Settings.GetLogotypeFilesPath(_leftLogotype.Item2), _rightLogotype.Item2, true);
        }

        public override IEnumerable<BaseVisitor> GetVisitors() => throw new ArgumentException("Have not visitables entities");
    }
}
