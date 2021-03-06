﻿namespace NServiceBus.Unicast.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MessageInterfaces;
    using MessageInterfaces.MessageMapper.Reflection;
    using NUnit.Framework;
    using Pipeline;
    using Testing;
    using PublishOptions = NServiceBus.PublishOptions;
    using ReplyOptions = NServiceBus.ReplyOptions;
    using SendOptions = NServiceBus.SendOptions;

    [TestFixture]
    public class MessageOperationsTests
    {
        [Test]
        public void When_sending_message_interface_should_set_interface_as_message_type()
        {
            var sendPipeline = new FakePipeline<IOutgoingSendContext>();
            var context = CreateContext(sendPipeline);

            MessageOperations.Send<IMyMessage>(context, m => { }, new SendOptions());

            Assert.That(sendPipeline.ReceivedContext.Message.MessageType, Is.EqualTo(typeof(IMyMessage)));
        }

        [Test]
        public void When_sending_message_class_should_set_class_as_message_type()
        {
            var sendPipeline = new FakePipeline<IOutgoingSendContext>();
            var context = CreateContext(sendPipeline);

            MessageOperations.Send<MyMessage>(context, m => { }, new SendOptions());

            Assert.That(sendPipeline.ReceivedContext.Message.MessageType, Is.EqualTo(typeof(MyMessage)));
        }

        [Test]
        public void When_replying_message_interface_should_set_interface_as_message_type()
        {
            var replyPipeline = new FakePipeline<IOutgoingReplyContext>();
            var context = CreateContext(replyPipeline);

            MessageOperations.Reply<IMyMessage>(context, m => { }, new ReplyOptions());

            Assert.That(replyPipeline.ReceivedContext.Message.MessageType, Is.EqualTo(typeof(IMyMessage)));
        }

        [Test]
        public void When_replying_message_class_should_set_class_as_message_type()
        {
            var replyPipeline = new FakePipeline<IOutgoingReplyContext>();
            var context = CreateContext(replyPipeline);

            MessageOperations.Reply<MyMessage>(context, m => { }, new ReplyOptions());

            Assert.That(replyPipeline.ReceivedContext.Message.MessageType, Is.EqualTo(typeof(MyMessage)));
        }

        [Test]
        public void When_publishing_event_interface_should_set_interface_as_message_type()
        {
            var publishPipeline = new FakePipeline<IOutgoingPublishContext>();
            var context = CreateContext(publishPipeline);

            MessageOperations.Publish<IMyMessage>(context, m => { }, new PublishOptions());

            Assert.That(publishPipeline.ReceivedContext.Message.MessageType, Is.EqualTo(typeof(IMyMessage)));
        }

        [Test]
        public void When_publishing_event_class_should_set_class_as_message_type()
        {
            var publishPipeline = new FakePipeline<IOutgoingPublishContext>();
            var context = CreateContext(publishPipeline);

            MessageOperations.Publish<MyMessage>(context, m => { }, new PublishOptions());

            Assert.That(publishPipeline.ReceivedContext.Message.MessageType, Is.EqualTo(typeof(MyMessage)));
        }

        IBehaviorContext CreateContext<TContext>(IPipeline<TContext> pipeline) where TContext : IBehaviorContext
        {
            var pipelineCache = new FakePipelineCache();
            pipelineCache.RegisterPipeline(pipeline);

            var context = new TestableMessageHandlerContext();
            context.Builder.Register<IMessageMapper>(() => new MessageMapper());
            context.Extensions.Set<IPipelineCache>(pipelineCache);

            return context;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public interface IMyMessage
        {
        }

        class MyMessage
        {
        }

        class FakePipelineCache : IPipelineCache
        {
            Dictionary<Type, object> pipelines = new Dictionary<Type, object>();

            public void RegisterPipeline<TContext>(IPipeline<TContext> pipeline) where TContext : IBehaviorContext
            {
                pipelines.Add(typeof(TContext), pipeline);
            }

            public IPipeline<TContext> Pipeline<TContext>() where TContext : IBehaviorContext
            {
                return pipelines[typeof(TContext)] as IPipeline<TContext>;
            }
        }

        class FakePipeline<TContext> : IPipeline<TContext> where TContext : IBehaviorContext
        {
            public TContext ReceivedContext { get; set; }

            public Task Invoke(TContext context)
            {
                ReceivedContext = context;
                return TaskEx.CompletedTask;
            }
        }
    }
}