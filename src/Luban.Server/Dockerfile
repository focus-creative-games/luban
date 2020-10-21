FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

WORKDIR /app/Luban.Common
COPY Luban.Common/*.csproj ./
COPY Luban.Common/Source ./Source
COPY Luban.Common/.editorconfig .
COPY nuget.config ./nuget.config

WORKDIR /app/Luban.Server.Common
COPY Luban.Server.Common/*.csproj ./
COPY Luban.Server.Common/Source ./Source
COPY nuget.config ./nuget.config

WORKDIR /app/Luban.Job.Common
COPY Luban.Job.Common/*.csproj ./
COPY Luban.Job.Common/Source ./Source
COPY nuget.config ./nuget.config

WORKDIR /app/Luban.Job.Cfg
COPY Luban.Job.Cfg/*.csproj ./
COPY Luban.Job.Cfg/Source ./Source
COPY nuget.config ./nuget.config

WORKDIR /app/Luban.Server
COPY Luban.Server/Luban.Server.csproj ./
COPY Luban.Server/.editorconfig .
COPY Luban.Server/Source ./Source
COPY nuget.config ./nuget.config

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS runtime
RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime && echo "Asia/Shanghai" > /etc/timezone
WORKDIR /app
COPY --from=build /app/Luban.Server/out ./
EXPOSE 8899/tcp
ENTRYPOINT ["/app/Luban.Server", "-p", "8899"]