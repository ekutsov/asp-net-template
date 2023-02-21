global using A2SEVEN.Infrastructure.Context;
global using AutoMapper;
global using A2SEVEN.Domain.Settings;

#if (authorization == JWT)
global using A2SEVEN.Infrastructure.Entities;
global using A2SEVEN.Domain.Interfaces;
global using A2SEVEN.Domain.DTO;
global using A2SEVEN.Domain.ViewModels;
#endif

global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Options;
global using System.IdentityModel.Tokens.Jwt;
global using System.Net;
global using System.Security.Cryptography;
global using System.Text;
global using A2SEVEN.Domain.Constants;
global using A2SEVEN.Domain.Exceptions;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;