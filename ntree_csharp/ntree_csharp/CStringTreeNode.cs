using System.IO;

namespace ntree_csharp
{
    class CStringTreeNode : CTreeNodeWriteable
    {
        public CStringTreeNode(string sStr)
        {
            m_sStr = sStr;
        }

        public CStringTreeNode(string sStr, CStringTreeNode pNode, ENodeCreate eFlag) : base(pNode, eFlag)
        {
            m_sStr = sStr;
        }

        public override int GetWidth()
        {
            return m_sStr.Length;
        }

        public override void WriteNode(StringWriter clSW)
        {
            clSW.Write(m_sStr);
        }

        readonly string m_sStr;
    }
}