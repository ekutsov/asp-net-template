global using Microsoft.AspNetCore.Mvc;
global using EKUTSOV.Domain.Exceptions;
global using System.Net;
global using System.Text.Json;
global using EKUTSOV.Infrastructure.Context;
global using Microsoft.EntityFrameworkCore;
global using EKUTSOV.Core.Services;
global using EKUTSOV.Domain.Settings;

#if (authorization == JWT)
global using EKUTSOV.Domain.DTO;
global using EKUTSOV.Domain.Interfaces;
global using EKUTSOV.Domain.ViewModels;
global using EKUTSOV.Infrastructure.Entities;
#endif

global using Microsoft.AspNetCore.Authorization;
global using EKUTSOV.API.Configuration;
global using EKUTSOV.Core.Configuration;

global using Microsoft.AspNetCore.Identity;
global using EKUTSOV.Core.Middlewares;
global using System.IdentityModel.Tokens.Jwt;
global using System.Text;
global using EKUTSOV.Domain.Constants;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;