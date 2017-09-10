using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliaryLibrary.EverythingNet
{
    public class Everything
    {
        /// <summary>
        /// 获取或设置一个指示[匹配路径]的值。
        /// </summary>
        public bool MatchPath
        {
            get
            {
                return EverythingApi.Everything_GetMatchPath();
            }
            set
            {
                EverythingApi.Everything_SetMatchPath(value);
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示是否[匹配大小写]。
        /// </summary>
        public bool MatchCase
        {
            get
            {
                return EverythingApi.Everything_GetMatchCase();
            }
            set
            {
                EverythingApi.Everything_SetMatchCase(value);
            }
        }

        /// <summary>
        ///获取或设置一个值，表示是否[匹配整个字]。
        /// </summary>
        public bool MatchWholeWord
        {
            get
            {
                return EverythingApi.Everything_GetMatchWholeWord();
            }
            set
            {
                EverythingApi.Everything_SetMatchWholeWord(value);
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示[启用正则表达式]。
        /// </summary>
        public bool EnableRegex
        {
            get
            {
                return EverythingApi.Everything_GetRegex();
            }
            set
            {
                EverythingApi.Everything_SetRegex(value);
            }
        }


        /// <summary>
        /// 重置此实例。
        /// </summary>
        public void Reset()
        {
            EverythingApi.Everything_Reset();
        }

        /// <summary>
        /// 搜索指定的关键字。
        /// </summary>
        /// <param name="keyWord">The key word.</param>
        /// <returns></returns>
        public IEnumerable<string> Search(string keyWord)
        {
            return Search(keyWord, 0, int.MaxValue);
        }

        /// <summary>
        ///搜索指定的关键字。
        /// </summary>
        /// <param name="keyWord">The key word.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="maxCount">The max count.</param>
        /// <returns></returns>
        public IEnumerable<string> Search(string keyWord, int offset, int maxCount)
        {
            if (string.IsNullOrEmpty(keyWord))
            {
                throw new ArgumentNullException("keyWord");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (maxCount < 0)
            {
                throw new ArgumentOutOfRangeException("maxCount");
            }

            EverythingApi.Everything_SetSearch(keyWord);
            EverythingApi.Everything_SetOffset(offset);
            EverythingApi.Everything_SetMax(maxCount);
            if (!EverythingApi.Everything_Query())
            {
                switch (EverythingApi.Everything_GetLastError())
                {
                    case StateCode.CreateThreadError:
                        throw new CreateThreadException();
                    case StateCode.CreateWindowError:
                        throw new CreateWindowException();
                    case StateCode.InvalidCallError:
                        throw new InvalidCallException();
                    case StateCode.InvalidIndexError:
                        throw new InvalidIndexException();
                    case StateCode.IPCError:
                        throw new IPCErrorException();
                    case StateCode.MemoryError:
                        throw new MemoryErrorException();
                    case StateCode.RegisterClassExError:
                        throw new RegisterClassExException();
                }
                yield break;
            }
            const int bufferSize = 256;
            StringBuilder buffer = new StringBuilder(bufferSize);
            for (int idx = 0; idx < EverythingApi.Everything_GetNumResults(); ++idx)
            {
                EverythingApi.Everything_GetResultFullPathName(idx, buffer, bufferSize);
                yield return buffer.ToString();
            }
        }
    }
}
