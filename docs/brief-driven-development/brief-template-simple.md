# Simple Brief Template (3-5 sentences is enough!)

## What
[1-2 sentences describing what you want to build]

## Why  
[1 sentence on why this matters or what problem it solves]

## Constraints (optional)
[Any NON-OBVIOUS requirements like performance limits or business rules]

## Risk Tolerance (optional)
[Low, Medium, or High - helps agent recommend the right approach]

## Starting Points (optional but helpful!)
[Vague hints about where to look - saves agent search time]

---

**What NOT to include** (too specific):
- Exact file paths ("look in Controllers/UserController.cs")
- Framework versions ("we use ASP.NET Core 8")  
- Specific class names ("use the RedisCacheService")

**What TO include** (helpful hints):
- General areas ("check how we do authentication")
- Keywords to search for ("we call them 'policies'")
- Similar features ("like our existing caching")
- Vague patterns ("we use attributes for this kind of thing")

**Why hints help**: The agent will search more efficiently with keywords rather than exploring the entire codebase blindly

The agent will:
- Discover your tech stack and patterns
- Research the codebase for you  
- Create the detailed documentation
- Present you with simple choices using Risk (🎯) and Effort (💪) measures

## Examples of Good Simple Briefs:

**Example 1:**
What: Add rate limiting to our API endpoints.
Why: Some users are making 1000s of requests/minute, affecting everyone else.
Constraints: Must stay under 50ms latency, fail open if Redis is down.
Starting Points: Look at our auth middleware, we use Redis for caching already.

**Example 2:**  
What: Create a dark mode for the admin dashboard.
Why: Users are complaining about eye strain during late night oncall shifts.
Starting Points: Check how we handle user preferences, look for "theme" in the codebase.

**Example 3:**
What: Add CSV export to all data tables.
Why: Users need to analyze data in Excel for quarterly reports.
Constraints: Must handle 100k+ rows without timing out or consuming too much memory.
Starting Points: We already have some export functionality, search for "export" or "download".