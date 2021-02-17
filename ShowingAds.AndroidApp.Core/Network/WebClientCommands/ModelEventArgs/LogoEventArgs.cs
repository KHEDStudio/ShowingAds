using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands.ModelEventArgs
{
    public class LogoEventArgs : EventArgs
    {
        public Guid Id { get; private set; }
        public string LogoPath { get; private set; }
        public bool IsLeft { get; private set; }

        public LogoEventArgs(Guid id, string logoPath, bool isLeft)
        {
            Id = id;
            LogoPath = logoPath ?? throw new ArgumentNullException(nameof(logoPath));
            IsLeft = isLeft;
        }
    }
}
