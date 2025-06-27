# Brief: Message Queue Infrastructure

We need asynchronous messaging between our microservices for several use cases:
- Order processing: Must guarantee exactly-once delivery and maintain order sequence
- Inventory updates: Need pub/sub to notify multiple services when stock changes
- Background jobs: Fire-and-forget tasks like sending emails and generating reports

Expecting 10K-100K messages per second at peak. Messages are typically small (< 10KB) but some report generation tasks might include 1MB payloads.

Must integrate well with .NET and our Azure infrastructure. Need to handle service failures gracefully - messages can't be lost.

Research both cloud-managed and self-hosted options.