using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface ICrawlable : ICreatureAction
    {
        public event Action<object> OnCrawlStart;
        public event Action<object> OnCrawlEnd;

        public bool IsCrawling { get; }
        public bool IsCrawlEntering { get; }
        public bool IsCrawlExiting { get; }

        public float CrawlMaxSpeed { get; }
        public float CrawlAcceleration { get; }

        public void Crawling();
    }
}
