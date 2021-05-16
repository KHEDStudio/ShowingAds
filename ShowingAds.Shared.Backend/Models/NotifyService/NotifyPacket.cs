using ShowingAds.Shared.Backend.Enums;
using System;
using System.Collections.Generic;

namespace ShowingAds.Shared.Backend.Models.NotifyService
{
    public class NotifyPacket
    {
        public IEnumerable<Guid> Recipients { get; private set; }
        public Operation Operation { get; private set; }
        public string ModelType { get; private set; }
        public string Model { get; private set; }

        public NotifyPacket(IEnumerable<Guid> recipients, Operation operation, string modelType, string model)
        {
            Recipients = recipients ?? throw new ArgumentNullException(nameof(recipients));
            Operation = operation;
            ModelType = modelType ?? throw new ArgumentNullException(nameof(modelType));
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }
    }
}
