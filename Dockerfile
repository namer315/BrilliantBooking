FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ERPBooking.csproj ./
RUN dotnet restore ERPBooking.csproj
COPY . ./
RUN dotnet publish ERPBooking.csproj -c Release --no-restore --no-self-contained -p:UseAppHost=false -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
RUN mkdir /data && chown $APP_UID:$APP_UID /data
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV Booking__DataPath=/data/reservations.json
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "ERPBooking.dll"]
