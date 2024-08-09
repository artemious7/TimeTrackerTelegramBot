using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

internal class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error running a function");
            try
            {
                await SetResponseStatusCodeTo500(context, ex);
            }
            // to avoid infinite recursion, just log the error
            catch (Exception ex2)
            {
                logger.LogError(ex2, "Error setting response status code");
            }
        }

        static async Task SetResponseStatusCodeTo500(FunctionContext context, Exception ex)
        {
            var request = await context.GetHttpRequestDataAsync();
            if (request == null)
            {
                return;
            }

            var response = request.CreateResponse();
            response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            await response.WriteStringAsync($"An error occurred while processing function '{context.FunctionDefinition.Name}'\r\n");
            await response.WriteStringAsync(ex.ToString());
        }
    }
}