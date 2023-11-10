using Resto.Front.Api.Attributes;
using Resto.Front.Api.Attributes.JetBrains;
using Resto.Front.Api.Data.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Resto.Front.Api.ReservationVisualPlugin
{
    /// <summary>
    /// Плагин для визуального отображения статуса столов в iiko
    /// </summary>
    [UsedImplicitly]
    [PluginLicenseModuleId(19011701)]
    public sealed class ReservationVisualPlugin : IFrontPlugin
    {
        private readonly Stack<IDisposable> subscriptions = new Stack<IDisposable>();

        public ReservationVisualPlugin()
        {
            PluginContext.Log.Info("Initializing ReservationVisualPlugin");
            // Подписываемся на ReservesViewer
            subscriptions.Push(new ReservesViewer());

            PluginContext.Log.Info("ReservationVisualPlugin started");
        }

        public void Dispose()
        {
            while (subscriptions.Any())
            {
                var subscription = subscriptions.Pop();
                try
                {
                    subscription.Dispose();
                }
                catch (RemotingException)
                {
                    // nothing to do with the lost connection
                }
            }

            PluginContext.Log.Info("SamplePlugin stopped");
        }

    }
}
