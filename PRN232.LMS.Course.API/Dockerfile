FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PRN232.LMS.Course.API/PRN232.LMS.Course.API.csproj", "PRN232.LMS.Course.API/"]
RUN dotnet restore "PRN232.LMS.Course.API/PRN232.LMS.Course.API.csproj"
COPY . .
WORKDIR "/src/PRN232.LMS.Course.API"
RUN dotnet build "PRN232.LMS.Course.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PRN232.LMS.Course.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PRN232.LMS.Course.API.dll"]
