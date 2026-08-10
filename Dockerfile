FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5193

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /build
COPY ["src/Poc.VerticalSlice.Application/Poc.VerticalSlice.Application.csproj", "src/Poc.VerticalSlice.Application/"]
COPY ["src/Poc.VerticalSlice.WebApi/Poc.VerticalSlice.WebApi.csproj", "src/Poc.VerticalSlice.WebApi/"]
RUN dotnet restore "src/Poc.VerticalSlice.WebApi/Poc.VerticalSlice.WebApi.csproj"
COPY . .
RUN dotnet build "src/Poc.VerticalSlice.WebApi/Poc.VerticalSlice.WebApi.csproj" \
    -c Release \
    -o /app/build

FROM build AS publish
RUN dotnet publish "src/Poc.VerticalSlice.WebApi/Poc.VerticalSlice.WebApi.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM base AS final
ARG USERNAME=dev
ARG USER_UID=1000
ARG USER_GID=1000
WORKDIR /app
COPY --from=publish /app/publish .
RUN groupadd --gid $USER_GID $USERNAME \
    && useradd --uid $USER_UID --gid $USER_GID -m $USERNAME
USER $USERNAME
ENTRYPOINT ["dotnet", "Poc.VerticalSlice.WebApi.dll"]