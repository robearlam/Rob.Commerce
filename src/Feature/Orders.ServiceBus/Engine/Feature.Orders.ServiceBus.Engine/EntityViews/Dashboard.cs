using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.SQL;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.EntityViews
{
    [PipelineDisplayName("Dashboard")]
    public class Dashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly GetListCountCommand _getCountCommand;

        public Dashboard(GetListCountCommand getListCountCommand)
        {
            _getCountCommand = getListCountCommand;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");

            if (entityView.Name != "ServiceBus")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<ServiceBusOrderPlacedPolicy>();
            var count = await _getCountCommand.Process(context.CommerceContext, pluginPolicy.OrderPlacedListName);
            var sentOrdersCount = await _getCountCommand.Process(context.CommerceContext, pluginPolicy.OrderSentListName);

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "ServiceBus Dashboard";

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ServiceBus Connection String",
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "END_POINT" // pluginPolicy.EndPoint
                });

            entityView.ChildViews.Add(
            new EntityView
            {
                ItemId = "localordersnotsent",
                Icon = pluginPolicy.Icon,
                DisplayName = "Local Orders pending send to ServiceBus",
                Properties = new List<ViewProperty> {
                                    new ViewProperty {Name = "ItemId", RawValue = "localordersnotsent", UiType = "EntityLink", IsHidden = true },
                                    new ViewProperty {Name = "QueueName", RawValue = pluginPolicy.OrderPlacedListName, UiType = "EntityLink" },
                                    new ViewProperty {Name = "Orders Count", RawValue = count, UiType = "EntityLink" }                                   

                }
            });


            entityView.ChildViews.Add(
            new EntityView
            {
                ItemId = "Total Orders sent to ServiceBus",
                Icon = pluginPolicy.Icon,
                DisplayName = "Total Orders sent to ServiceBus",
                Properties = new List<ViewProperty> {
                                    new ViewProperty {Name = "ItemId", RawValue = "localordersnotsent", UiType = "EntityLink", IsHidden = true },
                                    new ViewProperty {Name = "QueueName", RawValue = pluginPolicy.OrderSentListName, UiType = "EntityLink" },
                                    new ViewProperty {Name = "Orders Count", RawValue = sentOrdersCount, UiType = "EntityLink" }

                }
            });

            //entityView.ChildViews.Add(
            //    new EntityView
            //    {
            //        ItemId = "123",
            //        Icon = pluginPolicy.Icon,
            //        DisplayName = "ServiceBus Queue",
            //        Properties = new List<ViewProperty> {
            //                new ViewProperty {Name = "ItemId", RawValue = "abc123", UiType = "EntityLink", IsHidden = true },
            //                new ViewProperty {Name = "QueueName", RawValue = "Orders", UiType = "EntityLink" },
            //                new ViewProperty {Name = "Status", RawValue = "1", UiType = "EntityLink" },
            //                new ViewProperty {Name = "MaxSize", RawValue = "10000000", UiType = "EntityLink" },
            //                new ViewProperty {Name = "Active Message Count", RawValue = GetMetricsAsync(), UiType = "EntityLink" },

            //        }
            //    });



            return entityView;
        }


        //static async Task<string> GetMetricsAsync()
        //{
        //    var tenantId = "91700184-c314-4dc9-bb7e-a411df456a1e"; // AAD Tenant
        //    var clientId = "467b3b2f-bc5c-4bae-9561-3110b46a05f7"; // AAD Web App ID. Do not use a native app
        //    var secret = "wLr7AunVoSWVenHEWhlP6ru3036uYTR9gEmsRoNva1A="; // Your generated secret                              
        //    var resourceId = $"subscriptions/7275e887-f14a-4d83-ba28-a72cfc5cae94/resourceGroups/mscommerce/providers/Microsoft.ServiceBus/namespaces/sitecore"; // resourceId can be taken when you select the namespace you intend to use in the portal and copy the url. Then delete everything before "subscriptions" and after the namespace name.                        
        //    //subscriptions/7275e887-f14a-4d83-ba28-a72cfc5cae94/resourcegroups/mscommerce"
        //    string entityName = "Orders";  // Queue or Topic name           
        //    string metricName = "ActiveMessages";  // Valid metrics "IncomingMessages,IncomingRequests,ActiveMessages,Messages,Size"            
        //    string aggregation = "Total"; // Valid aggregations: Total and Average

        //    // Create new Metrics token and Management client.
        //    var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, secret);
        //    MonitorManagementClient monitoringClient = new MonitorManagementClient(serviceCreds);

        //    var metricDefinitions = monitoringClient.MetricDefinitions.List(resourceId);
        //    //if (metricDefinitions.FirstOrDefault(
        //    //        metric => string.Equals(metric.Name.Value, metricName, StringComparison.InvariantCultureIgnoreCase)) == null)
        //    //{
        //    //    //Console.WriteLine("Invalid metric");
        //    //    //return;
        //    //}

        //    string startDate = DateTime.Now.AddHours(-1).ToString("o");
        //    string endDate = DateTime.Now.ToString("o");
        //    string timeSpan = startDate + "/" + endDate;
        //    ODataQuery<MetadataValue> odataFilterMetrics = new ODataQuery<MetadataValue>($"EntityName eq '{entityName}'");

        //    // Use this as quick and easy way to understand what metrics are emitted and what to query for. 
        //    // When looking for the count and size of an entity the only supported way is using total and 1 minute time slices.
        //    // Accessing those metrics via code is mostly for auto scaling purposes on sender and receiver side.
        //    //Response metrics1 = monitoringClient.Metrics.List(resourceUri: resourceId, metricnames: metricName, odataQuery: odataFilterMetrics, timespan: timeSpan, aggregation: aggregation, interval: TimeSpan.FromMinutes(1));
        //    //var metrics1 = await monitoringClient.Metrics.ListAsync(resourceUri: resourceId, metricnames: metricName, odataQuery: odataFilterMetrics, timespan: timeSpan, aggregation: aggregation, interval: TimeSpan.FromMinutes(1));
        //    //Console.WriteLine(JsonConvert.SerializeObject(metrics1, Newtonsoft.Json.Formatting.Indented));

        //    // Use this to get a list output to your console                        
        //    Response metrics = await monitoringClient.Metrics.ListAsync(resourceId, odataFilterMetrics, timeSpan, TimeSpan.FromMinutes(1), metricName, aggregation);
        //    //return EnumerateMetrics(metrics, resourceId, entityName);

        //    var numRecords = 0;
        //    var maxRecords = 1;
        //    {

        //        foreach (var metric in metrics.Value)
        //        {

        //            foreach (var timeSeries in metric.Timeseries)
        //            {
        //                // Use Average and multiplier for bigger time ranges than one minute and when observing bigger time ranges than 5 minutes.
        //                // Use Total for short time ranges and 1 minute interval for observing e.g. one hour worth of data and decide to automatically scale receivers or senders.
        //                foreach (var data in timeSeries.Data)
        //                {

        //                    //Console.WriteLine(
        //                    //    "{0}\t{1}\t{2}\t{3}", entityName,
        //                    //    metric.Name.Value,
        //                    //    metric.Name.LocalizedValue,
        //                    //    data.Total);
        //                    numRecords++;
        //                    if (numRecords >= maxRecords)
        //                    {
        //                        return data.Total.ToString();
        //                    }
        //                    return data.Total.ToString();

        //                }
        //            }

        //        }
        //        return "unable to get message count at this time";
        //    }

        //}
        //private static String EnumerateMetrics(Response metrics, string armId, string entityName, int maxRecords = 1)
        //{
        //    //var numRecords = 0;
        //    //{

        //    //    foreach (var metric in metrics.Value)
        //    //    {
                    
        //    //        foreach (var timeSeries in metric.Timeseries)
        //    //        {
        //    //            // Use Average and multiplier for bigger time ranges than one minute and when observing bigger time ranges than 5 minutes.
        //    //            // Use Total for short time ranges and 1 minute interval for observing e.g. one hour worth of data and decide to automatically scale receivers or senders.
        //    //            foreach (var data in timeSeries.Data)
        //    //            {

        //    //                //Console.WriteLine(
        //    //                //    "{0}\t{1}\t{2}\t{3}", entityName,
        //    //                //    metric.Name.Value,
        //    //                //    metric.Name.LocalizedValue,
        //    //                //    data.Total);
        //    //                numRecords++;
        //    //                if (numRecords >= maxRecords)
        //    //                {
        //    //                    return data.Total.ToString();
        //    //                }
        //    //                return data.Total.ToString();

        //    //            }
        //    //        }

        //    //    }
        //    //    return "unable to get message count at this time";
        //    //}
        //}

    }
}
