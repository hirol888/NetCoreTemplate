﻿using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;

namespace NetCoreTemplate.WebApi.Configuration {
  public static class AutoMapperExtensions {
    public static void ConfigureAutomapper(this IServiceCollection services,
      Action<IMapperConfigurationExpression> action) {
      services.AddAutoMapper(action, DependencyContext.Default);
    }
  }
}
