FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

WORKDIR /app/Luban.Common
COPY Luban.Common/*.csproj ./
COPY Luban.Common/.editorconfig ./

WORKDIR /app/Luban.ClientServer.Common
COPY Luban.ClientServer.Common/*.csproj ./
COPY nuget.config ./nuget.config

WORKDIR /app/Luban.Job.Common
COPY Luban.Job.Common/*.csproj ./
COPY nuget.config ./nuget.config

WORKDIR /app/Luban.Job.Cfg
COPY Luban.Job.Cfg/*.csproj ./
COPY nuget.config ./nuget.config

WORKDIR /app/Luban.Job.Proto
COPY Luban.Job.Proto/*.csproj ./
COPY nuget.config ./nuget.config

WORKDIR /app/Luban.Job.Db
COPY Luban.Job.Db/*.csproj ./
COPY nuget.config ./nuget.config

WORKDIR /app/Luban.ClientServer
COPY Luban.ClientServer/Luban.ClientServer.csproj ./
COPY Luban.ClientServer/.editorconfig .
COPY nuget.config ./nuget.config

RUN dotnet restore


WORKDIR /app/Luban.Common
COPY Luban.Common/Source ./Source

WORKDIR /app/Luban.ClientServer.Common
COPY Luban.ClientServer.Common/Source ./Source

WORKDIR /app/Luban.Job.Common
COPY Luban.Job.Common/Source ./Source

WORKDIR /app/Luban.Job.Cfg
COPY Luban.Job.Cfg/Source ./Source

WORKDIR /app/Luban.Job.Proto
COPY Luban.Job.Proto/Source ./Source

WORKDIR /app/Luban.Job.Db
COPY Luban.Job.Db/Source ./Source

WORKDIR /app/Luban.ClientServer
COPY Luban.ClientServer/Source ./Source
COPY Luban.Server/Templates ./Templates

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/Luban.ClientServer/out ./
EXPOSE 8899/tcp
ENTRYPOINT ["/app/Luban.ClientServer", "-p", "8899"]