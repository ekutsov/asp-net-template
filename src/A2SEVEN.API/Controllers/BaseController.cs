namespace A2SEVEN.API.Controllers;

public abstract class BaseController<TService> : ControllerBase
{
    protected TService _service;

    protected BaseController(TService service)
    {
        _service = service;
    }
}