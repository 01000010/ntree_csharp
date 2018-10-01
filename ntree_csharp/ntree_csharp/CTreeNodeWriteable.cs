using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ntree_csharp
{
    class CTreeNodeWriteable : CTreeNode
    {
        enum EWriteCommand { EWRITE_NOTHING, EWRITE_DATA, EWRITE_FORK, EWRITE_BRANCH }

        public CTreeNodeWriteable()
        {
            m_sWidth = 0;
            m_sStart = 0;
            m_sFork = 0;
            m_sLength = 0;
            m_eCommand = EWriteCommand.EWRITE_NOTHING;
        }

        public CTreeNodeWriteable(CTreeNode pNode, ENodeCreate eFlag) : base(pNode, eFlag)
        {
            m_eCommand = EWriteCommand.EWRITE_NOTHING;
        }

        public new CTreeNodeWriteable RightBrother()
        {
            return (CTreeNodeWriteable)base.RightBrother();
        }

        public new CTreeNodeWriteable LeftBrother()
        {
            return (CTreeNodeWriteable)base.LeftBrother();
        }

        public new CTreeNodeWriteable FirstChild()
        {
            return (CTreeNodeWriteable)base.FirstChild();
        }

        public new CTreeNodeWriteable LastChild()
        {
            return (CTreeNodeWriteable)base.LastChild();
        }

        public virtual int GetWidth() { return 0; }
        public virtual void WriteNode(StringWriter clSW) { }

        int InitWrite(int iSpace, int iDeep)
        {
            int iLength = GetWidth();
            int iChildWidth = 0;
            int iMaxDeep = ++iDeep;
            CTreeNodeWriteable pFirst = FirstChild();

            if (pFirst != null)
            {
                CTreeNodeWriteable pChild = pFirst;
                CTreeNodeWriteable pLast = pFirst;

                while (pChild != null)
                {
                    iMaxDeep = Math.Max(iMaxDeep, pChild.InitWrite(iSpace, iDeep));
                    iChildWidth += (pChild.m_sWidth + iSpace);
                    pLast = pChild;
                    pChild = pChild.RightBrother();
                }

                iChildWidth -= iSpace;

                int iFork = ((pFirst.m_sFork + (iChildWidth - (pLast.m_sWidth - pLast.m_sFork))) / 2);
                int iStart = (iFork - (iLength - 1) / 2);
                int iWidth = iChildWidth;

                if (iStart < 0)
                {
                    iFork -= iStart;
                    iWidth -= iStart;

                    for (CTreeNodeWriteable pFirstChild = FirstChild(); pFirstChild != null; pFirstChild = pFirstChild.FirstChild())
                    {
                        pFirstChild.m_sWidth = (short)(pFirstChild.m_sWidth - iStart);
                        pFirstChild.m_sFork = (short)(pFirstChild.m_sFork - iStart);
                        pFirstChild.m_sStart = (short)(pFirstChild.m_sStart - iStart);
                    }

                    iStart = 0;
                }

                int iPad = iStart + iLength - iWidth;

                if (iPad > 0)
                {
                    iWidth += iPad;

                    for (CTreeNodeWriteable pLastChild = LastChild(); pLastChild != null; pLastChild = pLastChild.LastChild())
                    {
                        pLastChild.m_sWidth = (short)(pLastChild.m_sWidth + iPad);
                    }
                }

                m_sFork = (short)iFork;
                m_sStart = (short)iStart;
                m_sWidth = (short)iWidth;
                m_sLength = (short)iLength;
            }
            else
            {
                m_sWidth = (short)iLength;
                m_sStart = 0;
                m_sLength = (short)iLength;
                m_sFork = (short)((iLength + 1) / 2 - 1);
            }

            return iMaxDeep;
        }

        static void WriteChars(StringWriter clSW, int iNum, Char cChar)
        {
            for (int i = 0; i < iNum; ++i)
            {
                clSW.Write(cChar);
            }
        }

        void WriteFork(StringWriter clSW, int iSpace)
        {
            if (m_sFork < 0)
            {
                WriteChars(clSW, m_sWidth + iSpace, ' ');
            }
            else
            {
                WriteChars(clSW, m_sFork, ' ');
                WriteChars(clSW, 1, '|');
                WriteChars(clSW, m_sWidth - m_sFork - 1 + iSpace, ' ');
            }
        }

        void WriteBranch(StringWriter clSW, int iSpace)
        {
            if (m_sFork < 0)
            {
                WriteChars(clSW, m_sWidth + iSpace, ' ');
            }
            else
            {
                WriteChars(clSW, m_sFork, IsFirstChild() ? ' ' : '¯');
                WriteChars(clSW, 1, '|');
                WriteChars(clSW, m_sWidth - m_sFork - 1 + iSpace, IsLastChild() ? ' ' : '¯');
            }
        }

        void WriteData(StringWriter clSW, int iSpace)
        {
            if (m_sFork < 0)
            {
                WriteChars(clSW, m_sWidth, ' ');
            }
            else
            {
                WriteChars(clSW, m_sStart, ' ');
                WriteNode(clSW);
                WriteChars(clSW, m_sWidth - m_sLength - m_sStart, ' ');
            }

            WriteChars(clSW, iSpace, ' ');
        }

        public String WriteTreeToString(int iSpace)
        {
            int iDeep = InitWrite(iSpace, 0);
            int iRow = 1;
            Queue<CTreeNodeWriteable> clQueue = new Queue<CTreeNodeWriteable>();
            StringBuilder clSB = new StringBuilder();
            EWriteCommand eLast = EWriteCommand.EWRITE_DATA;

            m_eCommand = EWriteCommand.EWRITE_DATA;
            clQueue.Enqueue(this);

            using (StringWriter clSW = new StringWriter(clSB))
            {
                while (clQueue.Any())
                {
                    CTreeNodeWriteable pNode = clQueue.Dequeue();
                    EWriteCommand eCom = pNode.m_eCommand;

                    if (eCom != eLast)
                    {
                        clSW.WriteLine();
                    }

                    switch (eCom)
                    {
                        case EWriteCommand.EWRITE_DATA:
                            if (pNode.m_eCommand != eLast)
                            {
                                ++iRow;
                            }

                            pNode.WriteData(clSW, iSpace);

                            if (iRow < iDeep)
                            {
                                if (!pNode.HasChild())
                                {
                                    pNode.m_sFork = -1;
                                }

                                pNode.m_eCommand = EWriteCommand.EWRITE_FORK;
                                clQueue.Enqueue(pNode);
                            }

                            break;

                        case EWriteCommand.EWRITE_FORK:
                            pNode.WriteFork(clSW, iSpace);

                            if (pNode.m_sFork < 0)
                            {
                                pNode.m_eCommand = EWriteCommand.EWRITE_BRANCH;
                                clQueue.Enqueue(pNode);
                            }
                            else
                            {
                                CTreeNodeWriteable pChild = pNode.FirstChild();
                                while (pChild != null)
                                {
                                    pChild.m_eCommand = EWriteCommand.EWRITE_BRANCH;
                                    clQueue.Enqueue(pChild);
                                    pChild = pChild.RightBrother();
                                }
                            }

                            break;

                        case EWriteCommand.EWRITE_BRANCH:
                            pNode.WriteBranch(clSW, iSpace);
                            pNode.m_eCommand = EWriteCommand.EWRITE_DATA;
                            clQueue.Enqueue(pNode);

                            break;
                    }

                    eLast = eCom;
                }
            }

            return clSB.ToString();
        }

        short m_sWidth;
        short m_sStart;
        short m_sLength;
        short m_sFork;
        EWriteCommand m_eCommand;
    }
}