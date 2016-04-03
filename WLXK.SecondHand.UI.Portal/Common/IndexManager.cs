using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Lucene.Net.Store;
using Lucene.Net.Index;
using System.IO;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;

namespace WLXK.SecondHand.UI.Portal.Common
{
    public class IndexManager
    {
        public readonly static IndexManager myManager = new IndexManager();
        private IndexManager()
        {
        }
        //定义一个队列
        private Queue<JobSerach> queue = new Queue<JobSerach>();
        //接收写入索引库的内容。
        public void Add(JobSerach job)
        {
            job.JobType = JobType.Add;
            queue.Enqueue(job);//将具体的内容插入队列.
        }

        /// <summary>
        /// 删除索引项
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            JobSerach job = new JobSerach();
            job.Id = id;
            job.JobType = JobType.Delete;
            queue.Enqueue(job);
        }
        //定义一个线程，将队列中的数据取出来插入索引库中.(这个方法在网站应用程序一启动时就应该调用，所以放在Global中的Application_Start中)
        public void Start()
        {
            Thread myThread = new Thread(InsertContent);
            myThread.IsBackground = true;
            myThread.Start();
        }
        //不断扫描队列看看是否有数据
        public void InsertContent()
        {

            while (true)
            {
                if (queue.Count > 0)
                {
                    try
                    {
                        InsertIndex();//开始插入索引库
                    }
                    catch (Exception)
                    {
                        //记录日志


                    }

                }
                else
                {
                    Thread.Sleep(3000);//如果队列中没有数据，让线程休息一会，防止CPU空转.
                }
            }
        }
        public void InsertIndex()
        {
            string indexPath = LunceneHelper.lucenePath;//注意和磁盘上文件夹的大小写一致，否则会报错。将创建的分词内容放在该目录下。
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());//指定索引文件(打开索引目录) FS指的是就是FileSystem
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
            while (queue.Count > 0)
            {
                JobSerach job = queue.Dequeue();//出队
                writer.DeleteDocuments(new Term("id", job.Id.ToString()));//删除索引库中原有的文档.
                if (job.JobType == JobType.Delete)//表示从队列中取出来的是删除任务.
                {
                    continue;
                }
                Document document = new Document();//表示一篇文档。
                //Field.Store.YES:表示是否存储原值。只有当Field.Store.YES在后面才能用doc.Get("number")取出值来.Field.Index. NOT_ANALYZED:不进行分词保存
                document.Add(new Field("Id", job.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                //Field.Index. ANALYZED:进行分词保存:也就是要进行全文的字段要设置分词 保存（因为要进行模糊查询）
                //Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS:不仅保存分词还保存分词的距离。
                document.Add(new Field("Title", job.Title, Field.Store.YES, Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS));

                document.Add(new Field("Price", job.Price.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                document.Add(new Field("ImageAddress", job.ImageAddress, Field.Store.YES, Field.Index.NOT_ANALYZED));

                document.Add(new Field("Content", job.Content, Field.Store.YES, Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS));

                document.Add(new Field("MaiDian", job.MaiDian, Field.Store.YES, Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS));
                writer.AddDocument(document);
            }
            writer.Close();//会自动解锁。
            directory.Close();//不要忘了Close，否则索引结果搜不到
        }
    }
}


public class JobSerach
{
    public int Id { get; set; }
    public string Title { get; set; }
    public double Price { get; set; }
    public string ImageAddress { get; set; }
    public string Content { get; set; }
    public string MaiDian { get; set; }
    public JobType JobType { get; set; }
}
//标记队列中信息要进行什么样的操作
public enum JobType
{
    Add, Update, Delete
}



