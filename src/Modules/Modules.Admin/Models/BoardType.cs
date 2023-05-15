using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Models
{
    public class BoardType
    {

        public BoardType(string boardType)
        {
            ReplyYN = boardType[0] == 'Y';
            FileUpoadYN = boardType[1] == 'Y';
            AdminYN = boardType[2] == 'Y';
            ReferenceURLYN = boardType[3] == 'Y';
            NoticeDTYN = boardType[4] == 'Y';
            SubjectURLYN = boardType[5] == 'Y';
            CommentYN = boardType[6] == 'Y';
            BranchUseYN = boardType[7] == 'Y';
        }
        /// <summary>
        /// 파일업로드 사용
        /// </summary>
        public bool ReplyYN { get; private set; }
        /// <summary>
        /// 문서작성일사용
        /// </summary>
       // public bool AllowSetDocDT { get; private set; }
        /// <summary>
        /// 제목URL
        /// </summary>
        public bool FileUpoadYN { get; private set; }
        /// <summary>
        /// 관리자용
        /// </summary>
        public bool AdminYN { get; private set; }
        /// <summary>
        /// 관련URL
        /// </summary>
        public bool ReferenceURLYN { get; private set; }

        /// <summary>
        /// 개시기한
        /// </summary>
        public bool NoticeDTYN { get; private set; }

        /// <summary>
        /// 열람범위
        /// </summary>
      //  public bool AllowSetViewGroup { get; private set; }
        /// <summary>
        /// 답변
        /// </summary>
        public bool SubjectURLYN { get; private set; }
        /// <summary>
        /// 댓글
        /// </summary>
        public bool CommentYN { get; private set; }
        /// <summary>
        /// 분류면사용
        /// </summary>
        public bool BranchUseYN { get; private set; }
    }
}


