using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PanGu;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace WLXK.SecondHand.UI.Portal.Common
{
    public class LunceneHelper
    {
        public static string lucenePath { get; set; }

        public LunceneHelper()
        {
            
        }

        #region old

        /// <summary>
        /// 创建索引，将数据库中的数据取出来给Lucene索引库
        /// </summary>
        public static void CreateContent(JobSerach search)
        {
            //string indexPath = lucenePath;//注意和磁盘上文件夹的大小写一致，否则会报错。将创建的分词内容放在该目录下。
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(lucenePath), new NativeFSLockFactory());//指定索引文件(打开索引目录) FS指的是就是FileSystem
            bool isUpdate = IndexReader.IndexExists(directory);//IndexReader:对索引进行读取的类。该语句的作用：判断索引库文件夹是否存在以及索引特征文件是否存在。
            if (isUpdate)
            {
                //同时只能有一段代码对索引库进行写操作。当使用IndexWriter打开directory时会自动对索引库文件上锁。
                //如果索引目录被锁定（比如索引过程中程序异常退出），则首先解锁（提示一下：如果我现在正在写着已经加锁了，但是还没有写完，这时候又来一个请求，那么不就解锁了吗？这个问题后面会解决）
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }
            IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isUpdate, Lucene.Net.Index.IndexWriter.MaxFieldLength.UNLIMITED);//向索引库中写索引。这时在这里加锁。

            writer.DeleteDocuments(new Term("Id", search.Id.ToString()));//删除索引库中原有的项.
            Document document = new Document();//表示一篇文档。
            //Field.Store.YES:表示是否存储原值。只有当Field.Store.YES在后面才能用doc.Get("number")取出值来.Field.Index. NOT_ANALYZED:不进行分词保存
            document.Add(new Field("Id", search.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            //Field.Index. ANALYZED:进行分词保存:也就是要进行全文的字段要设置分词 保存（因为要进行模糊查询）
            //Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS:不仅保存分词还保存分词的距离。
            document.Add(new Field("Title", search.Title, Field.Store.YES, Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS));

            document.Add(new Field("Price", search.Price.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            document.Add(new Field("ImageAddress", search.ImageAddress, Field.Store.YES, Field.Index.NOT_ANALYZED));

            document.Add(new Field("Content", search.Content, Field.Store.YES, Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS));

            document.Add(new Field("MaiDian", search.MaiDian, Field.Store.YES, Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS));

            writer.AddDocument(document);

            writer.Close();//会自动解锁。
            directory.Close();//不要忘了Close，否则索引结果搜不到

        } 
        #endregion

        //搜索
        public static List<JobSerach> SearchContent(string kw)
        {//string indexPath = lucenePath;//最好将该项放在配置文件中。
            kw = kw.ToLower();
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(lucenePath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            //搜索条件
            PhraseQuery query = new PhraseQuery();
            foreach (string word in SplitWord(kw))//将用户输入的搜索内容进行了盘古分词、
            {
                query.Add(new Term("Title", word));
                //query.Add(new Term("Content", word));
                //query.Add(new Term("MaiDian", word));
            }
            query.SetSlop(100);//多个查询条件的词之间的最大距离.在文章中相隔太远 也就无意义.（例如 “大学生”这个查询条件和"简历"这个查询条件之间如果间隔的词太多也就没有意义了。）
            //TopScoreDocCollector是盛放查询结果的容器
            TopScoreDocCollector collector = TopScoreDocCollector.create(1000, true);
            searcher.Search(query, null, collector);//根据query查询条件进行查询，查询结果放入collector容器
            ScoreDoc[] docs = collector.TopDocs(0, collector.GetTotalHits()).scoreDocs;//得到所有查询结果中的文档,GetTotalHits():表示总条数   TopDocs(300, 20);//表示得到300（从300开始），到320（结束）的文档内容.       //可以用来实现分页功能

            List<JobSerach> list = new List<JobSerach>();
            for (int i = 0; i < docs.Length; i++)
            {
                //
                //搜索ScoreDoc[]只能获得文档的id,这样不会把查询结果的Document一次性加载到内存中。降低了内存压力，需要获得文档的详细内容的时候通过searcher.Doc来根据文档id来获得文档的详细内容对象Document.
                int docId = docs[i].doc;//得到查询结果文档的id（Lucene内部分配的id）
                Document doc = searcher.Doc(docId);//找到文档id对应的文档详细信息
                JobSerach result = new JobSerach();
                result.Title = Highlight(kw, doc.Get("Title"));
                result.Id = Convert.ToInt32(doc.Get("Id"));
                result.ImageAddress = doc.Get("ImageAddress");
                result.MaiDian = doc.Get("MaiDian");
                result.Price = double.Parse(doc.Get("Price"));
                result.Content = doc.Get("Content");
                list.Add(result);
            }
            return list;
        }

        //搜索包含关键词
        public static List<JobSerach> SearchContent(string kw,int index,int skipCount)
        {
            //string indexPath = lucenePath;//最好将该项放在配置文件中。
            kw = kw.ToLower();
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(lucenePath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            //搜索条件
            PhraseQuery query = new PhraseQuery();
            foreach (string word in SplitWord(kw))//将用户输入的搜索内容进行了盘古分词、
            {
                query.Add(new Term("Title", word));
                //query.Add(new Term("Content", word));
                //query.Add(new Term("MaiDian", word));
            }
            query.SetSlop(100);//多个查询条件的词之间的最大距离.在文章中相隔太远 也就无意义.（例如 “大学生”这个查询条件和"简历"这个查询条件之间如果间隔的词太多也就没有意义了。）
            //TopScoreDocCollector是盛放查询结果的容器
            TopScoreDocCollector collector = TopScoreDocCollector.create(1000, true);
            searcher.Search(query, null, collector);//根据query查询条件进行查询，查询结果放入collector容器
            ScoreDoc[] docs = collector.TopDocs(index-1, skipCount).scoreDocs;//得到所有查询结果中的文档,GetTotalHits():表示总条数   TopDocs(300, 20);//表示得到300（从300开始），到320（结束）的文档内容.       //可以用来实现分页功能

            List<JobSerach> list = new List<JobSerach>();
            for (int i = 0; i < docs.Length; i++)
            {
                //
                //搜索ScoreDoc[]只能获得文档的id,这样不会把查询结果的Document一次性加载到内存中。降低了内存压力，需要获得文档的详细内容的时候通过searcher.Doc来根据文档id来获得文档的详细内容对象Document.
                int docId = docs[i].doc;//得到查询结果文档的id（Lucene内部分配的id）
                Document doc = searcher.Doc(docId);//找到文档id对应的文档详细信息
                JobSerach result = new JobSerach();
                result.Title = Highlight(kw, doc.Get("Title"));
                result.Id = Convert.ToInt32(doc.Get("Id"));
                result.ImageAddress = doc.Get("ImageAddress");
                result.MaiDian = doc.Get("MaiDian");
                result.Price = double.Parse(doc.Get("Price"));
                result.Content = doc.Get("Content");
                list.Add(result);
            }
            return list;
        }

        /// <summary>
        /// 对用户输入的搜索的条件进行分词
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string[] SplitWord(string str)
        {
            List<string> list = new List<string>();
            Analyzer analyzer = new PanGuAnalyzer();//指定盘古分词
            TokenStream tokenStream = analyzer.TokenStream("", new StringReader(str));//
            Lucene.Net.Analysis.Token token = null;
            while ((token = tokenStream.Next()) != null)
            {
                list.Add(token.TermText());
            }
            return list.ToArray();
        }
        /// <summary>
        /// 对搜索的关键词高亮显示
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string Highlight(string keyword, string content)
        {
            //创建HTMLFormatter,参数为高亮单词的前后缀 
            PanGu.HighLight.SimpleHTMLFormatter simpleHTMLFormatter =
                   new PanGu.HighLight.SimpleHTMLFormatter("<font color='red'>",

"</font>");
            //创建 Highlighter ，输入HTMLFormatter 和 盘古分词对象Semgent 
            PanGu.HighLight.Highlighter highlighter =
                            new PanGu.HighLight.Highlighter(simpleHTMLFormatter,
                            new Segment());
            //设置每个摘要段的字符数 
            highlighter.FragmentSize = 100;
            //获取最匹配的摘要段 
            return highlighter.GetBestFragment(keyword, content);
        }
    }




}