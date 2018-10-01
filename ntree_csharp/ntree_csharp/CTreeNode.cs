namespace ntree_csharp
{
    class CTreeNode
    {
        public enum ENodeCreate { ENODE_FIRST_CHILD, ENODE_LAST_CHILD, ENODE_RIGHT_BROTHER, ENODE_LEFT_BROTHER };

        public CTreeNode()
        {
            m_pRightBrother = this;
            m_pLeftBrother = this;
            m_pChild = null;
            m_pParent = null;
        }

        public CTreeNode(CTreeNode pNode, ENodeCreate eFlag)
        {
            CTreeNode pPrev, pPost;
            m_pChild = null;

            switch (eFlag)
            {
                case ENodeCreate.ENODE_FIRST_CHILD:
                    m_pParent = pNode;

                    if (pNode.m_pChild != null)
                    {
                        pPost = pNode.m_pChild;
                        pPrev = pNode.m_pChild.m_pLeftBrother;
                        pPrev.m_pRightBrother = this;
                        pPost.m_pLeftBrother = this;
                        m_pRightBrother = pPost;
                        m_pLeftBrother = pPrev;
                        pNode.m_pChild = this;
                    }
                    else
                    {
                        m_pLeftBrother = this;
                        m_pRightBrother = this;
                        pNode.m_pChild = this;
                    }

                    break;

                case ENodeCreate.ENODE_LAST_CHILD:
                    m_pParent = pNode;

                    if (pNode.m_pChild != null)
                    {
                        pPost = pNode.m_pChild;
                        pPrev = pNode.m_pChild.m_pLeftBrother;
                        pPrev.m_pRightBrother = this;
                        pPost.m_pLeftBrother = this;
                        m_pRightBrother = pPost;
                        m_pLeftBrother = pPrev;
                    }
                    else
                    {
                        m_pLeftBrother = this;
                        m_pRightBrother = this;
                        pNode.m_pChild = this;
                    }

                    break;

                case ENodeCreate.ENODE_RIGHT_BROTHER:
                    m_pParent = pNode.m_pParent;
                    m_pRightBrother = pNode.m_pRightBrother;
                    m_pLeftBrother = pNode;
                    pNode.m_pRightBrother.m_pLeftBrother = this;
                    pNode.m_pRightBrother = this;

                    break;

                case ENodeCreate.ENODE_LEFT_BROTHER:
                    m_pParent = pNode.m_pParent;
                    m_pLeftBrother = pNode.m_pLeftBrother;
                    m_pRightBrother = pNode;
                    pNode.m_pLeftBrother.m_pRightBrother = this;
                    pNode.m_pLeftBrother = this;

                    if (m_pParent != null && m_pParent.m_pChild == pNode)
                    {
                        m_pParent.m_pChild = this;
                    }

                    break;
            }
        }

        public void DeleteChildren()
        {
            CTreeNode pFirst, pCur, pNext;

            if (m_pChild == null) return;

            pFirst = pCur = m_pChild;
            m_pChild = null;

            do
            {
                pNext = pCur.m_pRightBrother;
                pCur.m_pLeftBrother = null;
                pCur.m_pRightBrother = null;
                pCur.m_pParent = null;
                pCur.DeleteChildren();

            } while (pNext != pFirst);

            return;
        }

        public void Delete()
        {
            if (m_pParent.m_pChild == this)
            {
                m_pParent.m_pChild = (m_pRightBrother == this) ? null : m_pRightBrother;
            }

            m_pRightBrother.m_pLeftBrother = m_pLeftBrother;
            m_pLeftBrother.m_pRightBrother = m_pRightBrother;
            m_pLeftBrother = null;
            m_pRightBrother = null;
            m_pParent = null;

            DeleteChildren();
        }

        public CTreeNode RightBrother()
        {
            return m_pRightBrother.m_pParent == null || m_pRightBrother.m_pParent.m_pChild == m_pRightBrother ? null : m_pRightBrother;
        }

        public CTreeNode LeftBrother()
        {
            return m_pParent == null || m_pParent.m_pChild == this ? null : m_pLeftBrother;
        }

        public CTreeNode FirstChild()
        {
            return m_pChild;
        }

        public CTreeNode LastChild()
        {
            return m_pChild?.m_pLeftBrother;
        }

        public bool IsRoot()
        {
            return m_pParent == null;
        }

        public bool IsLastChild()
        {
            return m_pParent == null || m_pParent.m_pChild == m_pRightBrother;
        }

        public bool IsFirstChild()
        {
            return m_pParent == null || m_pParent.m_pChild == this;
        }

        public bool HasChild()
        {
            return m_pChild != null;
        }

        int MaxDepth()
        {
            int iDepth, iMax = 1;
            CTreeNode pChild = FirstChild();

            while (pChild != null)
            {
                iDepth = pChild.MaxDepth() + 1;

                if (iDepth > iMax)
                {
                    iMax = iDepth;
                }
            }

            return iMax;
        }

        CTreeNode m_pRightBrother;
        CTreeNode m_pLeftBrother;
        CTreeNode m_pChild;
        CTreeNode m_pParent;
    }
}