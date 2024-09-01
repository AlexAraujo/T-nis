[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly OpenAIAPI _openAiApi;

    public TestController(OpenAIAPI openAiApi)
    {
        _openAiApi = openAiApi;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_openAiApi != null);
    }
}