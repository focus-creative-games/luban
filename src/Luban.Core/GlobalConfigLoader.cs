﻿using System.Text;
using System.Text.Json;
using Luban.RawDefs;
using Luban.Schema;
using Luban.Utils;

namespace Luban;

public class GlobalConfigLoader : IConfigLoader
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private string _curDir;

    public GlobalConfigLoader()
    {

    }


    private class Group
    {
        public List<string> Names { get; set; }

        public bool Default { get; set; }
    }

    private class SchemaFile
    {
        public string FileName { get; set; }

        public string Type { get; set; }
    }

    private class Target
    {
        public string Name { get; set; }

        public string Manager { get; set; }

        public List<string> Groups { get; set; }

        public string TopModule { get; set; }
    }

    private class LubanConf
    {
        public List<Group> Groups { get; set; }

        public List<SchemaFile> SchemaFiles { get; set; }

        public string DataDir { get; set; }

        public List<Target> Targets { get; set; }
    }

    public LubanConfig Load(string fileName)
    {
        s_logger.Debug("load config file:{}", fileName);
        _curDir = Directory.GetParent(fileName).FullName;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        var globalConf = JsonSerializer.Deserialize<LubanConf>(File.ReadAllText(fileName, Encoding.UTF8), options);

        var configFileName = Path.GetFileName(fileName);
        var dataInputDir = Path.Combine(_curDir, globalConf.DataDir);
        List<RawGroup> groups = globalConf.Groups.Select(g => new RawGroup() { Names = g.Names, IsDefault = g.Default }).ToList();
        List<RawTarget> targets = globalConf.Targets.Select(t => new RawTarget() { Name = t.Name, Manager = t.Manager, Groups = t.Groups, TopModule = t.TopModule }).ToList();

        List<SchemaFileInfo> importFiles = new();
        foreach (var schemaFile in globalConf.SchemaFiles)
        {
            if (string.IsNullOrEmpty(schemaFile.Type))
            {
                var fullPath = Path.Combine(_curDir, schemaFile.FileName);
                if (!Directory.Exists(fullPath))
                {
                    throw new Exception($"{configFileName} schemal 文件错误: 目录'{fullPath}'不存在");
                }
            }
            string fileOrDirectory = Path.Combine(_curDir, schemaFile.FileName);
            foreach (var subFile in FileUtil.GetFileOrDirectory(fileOrDirectory))
            {
                importFiles.Add(new SchemaFileInfo() { FileName = subFile, Type = schemaFile.Type });
            }
        }
        return new LubanConfig()
        {
            ConfigFileName = configFileName,
            InputDataDir = dataInputDir,
            Groups = groups,
            Targets = targets,
            Imports = importFiles,
        };
    }

}
