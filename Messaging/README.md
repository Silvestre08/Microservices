# Messaging in .net with mass transit

Asynchronous messaging is a communication method that replaces traditional http request response pattern, and aims to resolve scalability problems in services oriented architectures. The traditional web apis, served over http have several problems that asynchronous messaging is trying to solve:

1. Tight coupling between components and services inside organzation. If service A calls servive B that then calls service C, they are tightly coupled. Service A needs B and C in order to even function. Usually, when one of the service changes, the other have to change too.
2. Solve cascading failures. When communication is synchronous, if service A call service b that then calls service C, we are introducing a lot of latency in the request. Not only that.. what happens if one of the services fails? The whole chain fails.. What if one of the services is slow? it will make the overall system very slow and unresponsive.
3. Restfull apis are limited by request response patterns. This is inflexible to implement complex workflows and event driven architectures.
4. Scalling of services. Synchronous services are very hard to scale, because even if we have many instances of them runnins, they are still dependent on all the other services.
5. Long running operations where request response patterns are not very well suited. Example: client asks for long running operation ans gets notifed when it is complete.

Messaging is not a silver bullet. Things to consider:

1. Messaging system introduces complexity in development.
2. Complexity in monitoring, troubleshooting, etc.
3. Harder to maintain infrastructrue. The infrastructure needs to be thought out for high availability.
4. Message ordering is challeging. Some systems can guarantee that at queue level, but on the overall system level can be very complex.
5. Message deduplication. Duplication of messages can occur due to network failures and retries, etc. So this requires idempotency implementation of the messaging system.
6. Message delivery guarantees (only once, at least once, etc)
7. Vendor lock in. Can be hard to switch messaging system in the future if we tightly couple the implementation details to any particular message broker. Use abstractions like mass transit can be good, that allow us to switch brokers.

## Basic concepts

So what is this pattern all about?
Instead of requests, we now have messages. The traditional http client becomes a sender (Producer/Publisher).
The server is the receiver or the consumer of the message. In this type of communication the sender does not wait for a response from the receiver.
It continues to do work while the receiver will handle the messages at a later time (components work independently, scale better and can hanlde large handle big message volume). Large volume processing can be achived by scalling the consumers horizontally making the system is more fault tolerant (if a service is unavailable mesages get queued and can be processed at a later time).

The services become loosely coupled. The send only cares about sending the message, without caring too much who will be receiving it.
This also allows services written in different technologies to integrate with each other in a neutral way.
If a service is very busy, a producer can still keep producing messages. We solve the temporal coupling problem. Messaages will be processed eventually.

### Messages

A message is a unit that carries information to a different component. It is made of:

1. Headers -> set by the messaging system or manually by us.
2. Payload -> business data to be broadcasted to different components.

In mass transit a message can be sent by creating a class, record or an instance of an interface. These classes should not have any behavior at all because they only transport data between components.
Messages are reference types.
Messages can also be initialized as anonymous types and be sent to publish and send methods.
Mass transit will inclue the headers, things like the content type of the message: most of the time "application/vnd.masstransit". It can also include the destination of the message, fault reasons and so on.
It is a best practice to keep messages in a separate library.

### Delivery modes/guarantees

Devivery modes or guarantees define how strongly a messaging system enforces the delivery of a message it handles. This refers to the assurance that the message sent will be delivered snd processed by some criteria.
This is an important design consideration as strong delivery guarantees need require more from the system (more checks and aknowledgements).
Each message is uniquely identified and their delivered state is maintained to ensure they are only delivered a certain amount of times. The three delivery modes:

1. At most once: fire and forget. In this mode, brokers deliver messages to consumers without any aknowledgement from the consumer. After the broker sends, even if the consumer crashes, the system will move on.
   This is a low latency and high throughput mode but messages can get lost.

2. At least once: the broker ensures messages are delivered at least once. The broker waits for the aknowledgment from the consumer. After the aknowledgment is received, the broker removes the the message from the queue. If the aknowledgment is not received by a certain amount of time, the Time-To-Live of the message, the broker will assume that the message was not processed. This mode ensures message delivery but can result in duplicated message processing (consumer crashes after processing the message but before sending an acknowledgment).

3. Exactly once: or transactional delivery. Some brokers support this feature where messages are delivered exactly once without duplicates or losses. In this mode, consumers process messages within a transaction, ensuring messages and processed and aknowledged or rolled back in case of failures. This mode incurs higher latency and reduced throughput. Sometimes it is achieved with additional patterns to the broker, like inbox, outbox and ensuring message idempotency.

### Topologies

A topology refers to the configuration and the structure of the message routing, including how exchanges, qeuues, bindings are set up and managed within the message broker.
It is how messages types are used to configure broker infrastructure, like broker topics and queues.
They are used to access broker capabilities like direct exchanges and routing keys.
Example: different exchanges will result in different routing behavior in rabbitmq.
Another examples: in mass transit a queue corresponds to a consumer or groups of consumers.

Types of topologies:

1. Automatic. When we leave up to mass transit to do the work of creating the underlying infrastructure on the transport.
2. Custom: mass transit will configure the topology at the recieve endpoint level. We can for example, when we do publish, a topology will populate a routing key of the message that is sent. This is a publi topology. We can have message topology too.

### Endpoints

In mass transit, endpoints are messaging addresses or destinations to which messages are sent. It is vital in organizing and routing messages between different application ports.
Mass transit configures endpoints for us based on conventions and should be what we use most of the time. However we can costumize it.
We can haveÇ

1. Receive endpoints: destination to receive messages. It is a well known queue where a consumer will listen and receive messages for processing. It is tied to a specific messagr queue or topic depending on the topic we are using.
2. Publish endpoints: publish events to all interested subscribers. We usually do not intereact directly.
3. Send endpoints. This representd the address where messages are sent explicitly without needing to know about consumers.

We can configure endpoints to specify different things like transport, subscriptions, concurrency and Prefetch limits, retry policies, etc

And endpoints is then somtehing that abstract the phisical messaging infrastrucure. It provides the mechanism for sending, receiving, or publish messages to a transport in a transport agnostic way.
