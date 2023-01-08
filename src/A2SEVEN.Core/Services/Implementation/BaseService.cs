namespace A2SEVEN.Core.Services;

public abstract class BaseService
{
    protected readonly AppDbContext _context;

    protected readonly IMapper _mapper;

    protected IConfigurationProvider _configurationProvider;

    public BaseService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _configurationProvider = mapper.ConfigurationProvider;
    }
}