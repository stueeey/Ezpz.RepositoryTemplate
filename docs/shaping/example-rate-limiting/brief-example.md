# Brief: Add API Rate Limiting

## What

Add rate limiting to our API to prevent abuse.

## Why

Some users are making 1000s of requests per minute which is degrading performance for everyone.

## Constraints

- Cannot add more than 50ms latency
- Must fail open if Redis is unavailable

## Risk Tolerance

Medium - We need protection but can't risk blocking legitimate users

## Context

- Normal users make 20-50 requests/minute
- Problem users spike to 1000s/minute

## Starting Points

- Look at how we handle authentication
- We already use Redis for something
- Check existing middleware patterns