# RFC: Message Queue Infrastructure

## Problem Statement

We need asynchronous messaging infrastructure to support three distinct use cases:
1. **Order processing**: Requires exactly-once delivery with strict ordering to prevent duplicate charges or out-of-sequence processing
2. **Inventory updates**: Pub/sub pattern where multiple services need real-time notifications of stock changes
3. **Background jobs**: Fire-and-forget tasks for emails and reports, where occasional loss is acceptable

The system must handle 10K-100K messages per second with messages up to 1MB (though most are <10KB). Integration with .NET and Azure is required, and message durability is critical for order processing and inventory updates.

## Assumptions & Clarifications

**Assumptions made:**

- "Exactly-once" for orders means idempotent processing with at-least-once delivery
- Order sequence is per-customer, not global ordering
- Background jobs can tolerate up to 1% loss rate
- No requirement for message replay/event sourcing

**Would benefit from clarification:**

- Is geographic distribution/DR needed?
- What's the acceptable latency for inventory updates?
- Do we need message TTL or automatic cleanup?
- Should failed messages go to dead letter queues or retry indefinitely?

## Research Summary

The message queue landscape divides into traditional brokers (RabbitMQ, ActiveMQ) and distributed streaming platforms (Kafka, Pulsar). Cloud providers now offer fully-managed services that eliminate operational overhead but create vendor lock-in. For our throughput requirements and Azure environment, the choice comes down to operational simplicity versus flexibility and cost.

Key findings:

- Managed services drastically reduce operational burden but cost more at scale
- Kafka excels at extreme throughput but is overkill for traditional messaging
- Azure Service Bus integrates best with existing Azure services
- RabbitMQ offers the best balance of features and flexibility for self-hosting

## Approaches

### Approach 1: Azure Service Bus

**What it is:** Microsoft's fully-managed enterprise message broker with native Azure integration.

**Pros:**

- Zero operational overhead - fully managed
- Native integration with Azure monitoring and services
- Built-in features like duplicate detection and message sessions
- Excellent .NET SDK with async/await patterns

**Cons:**

- Vendor lock-in to Azure
- Higher cost at scale (~$700/month for 100M messages)
- Limited to ~1M messages/second maximum
- Cannot run on-premises

**Best for:** Teams prioritizing operational simplicity and already invested in Azure

### Approach 2: RabbitMQ

**What it is:** Popular open-source message broker with flexible routing and broad language support.

**Pros:**

- Complete control and deployment flexibility
- Lower cost at scale when self-hosted
- Rich routing with exchanges and bindings
- Can run anywhere - on-prem, containers, any cloud

**Cons:**

- Requires operational expertise
- Manual scaling and monitoring setup
- Performance degrades with deep queues
- No built-in geo-replication

**Best for:** Teams needing flexibility or multi-cloud deployment

### Approach 3: Apache Kafka

**What it is:** Distributed event streaming platform designed for extreme throughput.

**Pros:**

- Handles millions of messages per second
- Permanent message storage with replay
- Built-in stream processing capabilities
- Horizontal scaling to thousands of nodes

**Cons:**

- High operational complexity
- Steep learning curve
- .NET client less mature than Java
- Overkill for simple queue patterns

**Best for:** Event streaming or extreme throughput requirements

## Comparison

| Factor               | Azure Service Bus | RabbitMQ    | Apache Kafka |
|----------------------|-------------------|-------------|--------------|
| Setup complexity     | Easy              | Medium      | Hard         |
| Operational burden   | None              | High        | Very High    |
| Cost at scale        | High              | Medium      | Medium       |
| .NET support         | Excellent         | Good        | Fair         |
| **Order processing** | Native sessions   | Manual impl | Complex      |
| **Pub/sub**          | Native topics     | Native      | Native       |
| **Exactly-once**     | Built-in          | Manual      | Transactions |
| **Mixed patterns**   | Excellent         | Good        | Poor         |
| Throughput limit     | 1M msg/s          | 100K msg/s  | 10M+ msg/s   |
| Azure integration    | Native            | Manual      | Manual       |

## Recommendation

**Recommended:** Azure Service Bus

Given your specific requirements - exactly-once delivery for orders, pub/sub for inventory, and mixed message patterns - Azure Service Bus is the clear winner. It natively supports all three use cases with:
- Message sessions for ordered delivery per customer
- Topics/subscriptions for pub/sub scenarios
- Built-in duplicate detection for exactly-once semantics
- Different tiers for different use cases (Premium for orders, Standard for background jobs)

The managed service eliminates operational overhead while providing enterprise features needed for order processing.

**Alternative if:** You need multi-cloud deployment or have strict budget constraints, choose RabbitMQ. However, you'll
need to implement exactly-once semantics yourself using deduplication tables.

**Avoid:** Kafka for this use case. While it excels at event streaming, it's unnecessarily complex for your mixed
messaging patterns and doesn't provide native exactly-once delivery for traditional request processing.

## Key Decisions Needed

- [ ] Confirm budget flexibility for managed service (~$700/month at scale)
- [ ] Verify no requirements for on-premises deployment
- [ ] Determine if message replay capability is needed (would favor Kafka)
- [ ] Confirm throughput won't exceed 1M messages/second

## References

- [Azure Service Bus Performance Guide](https://docs.microsoft.com/azure/service-bus-messaging/service-bus-performance-improvements)
- [RabbitMQ vs Azure Service Bus Comparison](https://www.cloudamqp.com/blog/rabbitmq-vs-azure-service-bus.html)
- [When to use Kafka vs RabbitMQ](https://aws.amazon.com/compare/the-difference-between-rabbitmq-and-kafka/)