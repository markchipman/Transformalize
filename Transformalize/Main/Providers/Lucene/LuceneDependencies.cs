namespace Transformalize.Main.Providers.Lucene {
    public class LuceneDependencies : AbstractConnectionDependencies {
        public LuceneDependencies(string processName)
            : base(
                new FalseTableQueryWriter(),
                new LuceneConnectionChecker(processName),
                new LuceneEntityRecordsExist(),
                new LuceneEntityDropper(),
                new LuceneEntityCreator(),
                new FalseViewWriter(),
                new LuceneTflWriter(),
                new FalseScriptRunner(),
                new FalseDataTypeService()) { }
    }
}