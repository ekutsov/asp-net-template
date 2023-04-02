global using Xunit;
global using FluentAssertions;
global using Moq;
global using System;
global using System.Threading.Tasks;

global using EKUTSOV.API.Controllers;

#if (authorization == JWT)
global using EKUTSOV.Domain.Interfaces;
global using EKUTSOV.Domain.DTO;
global using EKUTSOV.Domain.ViewModels;
global using EKUTSOV.UnitTests.Shared.Constants;
#endif

global using EKUTSOV.Domain.Exceptions;
global using EKUTSOV.Domain.Constants;
global using Microsoft.AspNetCore.Mvc;