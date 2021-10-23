using System.Collections.Generic;
using UnityEngine;

namespace Mirage.Profiler
{
    internal class Profiler
    {
        #region Fields

        public Dictionary<int, NetworkDiagnostics.MessageInfo> InboundMessages;
        public Dictionary<int, NetworkDiagnostics.MessageInfo> OutboundMessages;

        #endregion

        #region Properties

        public bool Record
        {
            get;
            set;
        }

        #endregion

        public Profiler()
        {
            NetworkDiagnostics.InMessageEvent += OnInMessageEvent;
            NetworkDiagnostics.OutMessageEvent += OnOutMessageEvent;
        }

        public void Update()
        {

        }

        #region Mirage Callbacks

        private void OnInMessageEvent(NetworkDiagnostics.MessageInfo info)
        {
            if (!Record) return;

            InboundMessages.Add(Time.frameCount, info);
        }

        private void OnOutMessageEvent(NetworkDiagnostics.MessageInfo info)
        {
            if (!Record) return;

            OutboundMessages.Add(Time.frameCount, info);
        }

        #endregion
    }
}
