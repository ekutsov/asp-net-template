global using Microsoft.AspNetCore.Mvc;
global using A2SEVEN.Domain.Exceptions;
global using System.Net;
global using System.Text.Json;
global using A2SEVEN.Infrastructure.Context;
global using Microsoft.EntityFrameworkCore;
global using A2SEVEN.Core.Services;
global using A2SEVEN.Domain.Settings;

#if (authorization == JWT)
global using A2SEVEN.Domain.DTO;
global using A2SEVEN.Domain.Interfaces;
global using A2SEVEN.Domain.ViewModels;
global using A2SEVEN.Infrastructure.Entities;
#endif

global using Microsoft.AspNetCore.Authorization;
global using A2SEVEN.API.Configuration;
global using A2SEVEN.Core.Configuration;

global using Microsoft.AspNetCore.Identity;
global using A2SEVEN.Core.Middlewares;
global using System.IdentityModel.Tokens.Jwt;
global using System.Text;
global using A2SEVEN.Domain.Constants;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;