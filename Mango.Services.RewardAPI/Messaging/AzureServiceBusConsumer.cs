using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Services;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer  
    {
        private readonly string serviceBusConnectionString;
        private readonly string OrderCreatedTopic;
        private readonly string OrderCreatedRewardsSubscription;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _RewardProcessor;
        private ServiceBusProcessor _registerUserProcessor;
        private readonly RewardsService _RewardsServic;
        public AzureServiceBusConsumer(IConfiguration configuration, RewardsService rewardsService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

            OrderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            OrderCreatedRewardsSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");

            var client = new ServiceBusClient(serviceBusConnectionString);

            _RewardProcessor = client.CreateProcessor(OrderCreatedTopic, OrderCreatedRewardsSubscription);
            _RewardsServic = rewardsService;

        }

        public async Task Start()
        {
            _RewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestRecived;
            _RewardProcessor.ProcessErrorAsync += ErrorHandler; 
            await _RewardProcessor.StartProcessingAsync();

            
        }

        
        public async Task Stop()
        {
            await _RewardProcessor.StopProcessingAsync();
            await _RewardProcessor.DisposeAsync();

            
        }


        private async Task OnNewOrderRewardsRequestRecived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                await _RewardsServic.UpdateRewards(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }

    
}
