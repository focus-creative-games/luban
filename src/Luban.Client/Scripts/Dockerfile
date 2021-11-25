FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

WORKDIR /app/Luban.Common
COPY Luban.Common/*.csproj ./
COPY Luban.Common/.editorconfig .
COPY nuget.config ./nuget.config

WORKDIR /app/Luban.Client
COPY Luban.Client/Luban.Client.csproj ./
COPY Luban.Client/.editorconfig .
COPY nuget.config ./nuget.config

RUN dotnet restore


WORKDIR /app/Luban.Common
COPY Luban.Common/Source ./Source

WORKDIR /app/Luban.Client
COPY Luban.Client/Source ./Source

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/Luban.Client/out ./

ENTRYPOINT ["/app/Luban.Client"]