using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface ICrawlable : IEntity
    {
        public bool IsCrawling { get; }
        public bool IsCrawlEntering { get; }
        public bool IsCrawlExiting { get; }

        public float CrawlMaxSpeed { get; }
        public float CrawlAcceleration { get; }

        public void Crawling();
    }
}
