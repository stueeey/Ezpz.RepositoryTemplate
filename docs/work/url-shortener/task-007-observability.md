# Task 007: Add Observability

[← Back to Task Summary](./task-summary.md)

**Size**: Small (2 files) - 🎯 Very Low Risk, 💪 Easy Effort  
**Dependencies**: Task 003 (API endpoints must be complete)

## Objective

Add structured logging, health checks, and basic metrics to monitor the URL shortener service.

## Implementation Steps

1. Add health checks in `Program.cs`
   ```csharp
   // Add health check services
   builder.Services.AddHealthChecks()
       .AddDbContextCheck<UrlShortenerDbContext>("database")
       .AddCheck("ready", () => HealthCheckResult.Healthy("Service is ready"));
   
   // Map health check endpoints
   app.MapHealthChecks("/health/live", new HealthCheckOptions
   {
       Predicate = _ => false // Run no checks, just return 200
   });
   
   app.MapHealthChecks("/health/ready", new HealthCheckOptions
   {
       Predicate = check => check.Name == "ready"
   });
   
   app.MapHealthChecks("/health", new HealthCheckOptions
   {
       ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
   });
   ```

2. Enhance logging in `Services/UrlShortenerService.cs`
   ```csharp
   public async Task<ShortUrlResponse> CreateShortUrlAsync(string originalUrl, DateTime? expiresAt = null)
   {
       using var activity = Activity.StartActivity("CreateShortUrl");
       
       try
       {
           _logger.LogInformation(
               "Creating short URL for {OriginalUrl} with expiration {ExpiresAt}", 
               originalUrl, 
               expiresAt);
           
           // ... existing implementation ...
           
           _logger.LogInformation(
               "Created short URL {ShortCode} for {OriginalUrl}", 
               shortUrl.ShortCode, 
               originalUrl);
           
           // Add custom metrics
           activity?.SetTag("shortcode", shortUrl.ShortCode);
           activity?.SetTag("has_expiration", expiresAt.HasValue);
           
           return response;
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, 
               "Failed to create short URL for {OriginalUrl}", 
               originalUrl);
           throw;
       }
   }
   
   public async Task IncrementClickCountAsync(string shortCode)
   {
       try
       {
           // ... existing implementation ...
           
           _logger.LogDebug(
               "Incremented click count for {ShortCode} to {ClickCount}", 
               shortCode, 
               shortUrl.ClickCount);
       }
       catch (Exception ex)
       {
           _logger.LogWarning(ex, 
               "Failed to increment click count for {ShortCode}", 
               shortCode);
           // Don't throw - this is fire-and-forget
       }
   }
   ```

3. Add middleware for request logging
   ```csharp
   // In Program.cs
   app.Use(async (context, next) =>
   {
       var sw = Stopwatch.StartNew();
       
       try
       {
           await next();
       }
       finally
       {
           var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
           
           logger.LogInformation(
               "Request {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
               context.Request.Method,
               context.Request.Path,
               context.Response.StatusCode,
               sw.ElapsedMilliseconds);
       }
   });
   ```

4. Add OpenTelemetry (optional enhancement)
   ```csharp
   // In Program.cs
   builder.Services.AddOpenTelemetry()
       .WithMetrics(metrics =>
       {
           metrics
               .AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddMeter("UrlShortener");
       })
       .WithTracing(tracing =>
       {
           tracing
               .AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddEntityFrameworkCoreInstrumentation();
       });
   ```

## Validation Criteria

- [ ] /health endpoint returns service status
- [ ] /health/live always returns 200 when service is running
- [ ] /health/ready checks database connectivity
- [ ] Structured logs include correlation IDs
- [ ] Request/response times are logged
- [ ] Errors include full context in logs
- [ ] Activities/spans created for tracing (if OpenTelemetry added)

## Notes

- Health checks enable Kubernetes/container orchestration
- Structured logging enables better searching in log aggregators
- Consider adding custom metrics for business KPIs (URLs created/hour)
- OpenTelemetry is optional but recommended for production
- Activity tags help with distributed tracing