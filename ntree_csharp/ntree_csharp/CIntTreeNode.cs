using System.IO;

namespace ntree_csharp
{
    class CIntTreeNode : CTreeNodeWriteable
    {
        public CIntTreeNode(int iVal)
        {
            m_iVal = iVal;
        }

        public CIntTreeNode(int iVal, CIntTreeNode pNode, ENodeCreate eFlag) : base(pNode, eFlag)
        {
            m_iVal = iVal;
        }

        public override int GetWidth()
        {
            return m_iVal.ToString().Length;
        }

        public override void WriteNode(StringWriter clSW)
        {
            clSW.Write(m_iVal.ToString());
        }

        readonly int m_iVal;
    }
}