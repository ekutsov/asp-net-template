global using EKUTSOV.Infrastructure.Context;
global using AutoMapper;
global using EKUTSOV.Domain.Settings;

#if (authorization == JWT)
global using EKUTSOV.Infrastructure.Entities;
global using EKUTSOV.Domain.Interfaces;
global using EKUTSOV.Domain.DTO;
global using EKUTSOV.Domain.ViewModels;
#endif

global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Options;
global using System.IdentityModel.Tokens.Jwt;
global using System.Net;
global using System.Security.Cryptography;
global using System.Text;
global using EKUTSOV.Domain.Constants;
global using EKUTSOV.Domain.Exceptions;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;