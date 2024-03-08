using System.Net;

using var httpListener = new HttpListener();

httpListener.Prefixes.Add("http://localhost:13348/");

httpListener.Start();

while (true)
{
    var requestContext = await httpListener.GetContextAsync();

    requestContext.Response.AddHeader("Content-Type", "text/html");

    using var streamWriter = new StreamWriter(requestContext.Response.OutputStream);
    
    switch (requestContext.Request.RawUrl)
    {
        case "/list":
        {
            var product = new
            {
                Name = "Potato",
                Quantity = 3
            };
            var link = "\"https://google.com/\"";
            streamWriter.Write(@$"<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <div>Product</div>
        <div>List</div>
        <span>Product</span>
        <span>List</span>
        <a href={link}>Google</a>
        <ol>
            <li>{product.Name} {product.Quantity}</li>
            <li>Second item</li>
            <li>Third item</li>
        </ol>
    </body>
</html>
    ");
            streamWriter.Flush();
            break;
        }
        case "/about":
        {
            streamWriter.Write(@"<!DOCTYPE html>
        <html>
        <head></head>
        <body>
            <div>This is an example of HttpListener.</div>
        </body>
        </html>
");
            
            
            await streamWriter.FlushAsync();
            break;
        }
        default:
        {
            requestContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            streamWriter.Write(@$"<!DOCTYPE html>
        <html>
        <head></head>
        <body>
            <div>{requestContext.Response.StatusCode}</div>
        </body>
        </html>
");
            break;
        }
    }
    
    
    
}











