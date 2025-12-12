import React, { useEffect, useRef, useState } from "react";
import "react-quill-new/dist/quill.snow.css";
import ReactQuill from "react-quill-new";

const RichTextEditor = ({
  value,
  onChange,
}: {
  value: string;
  onChange: (content: string) => void;
}) => {
  const [editorValue, setEditorValue] = useState(value || "");
  const quillRef = useRef(false);

  useEffect(() => {
    if (!quillRef.current) {
      quillRef.current = true;

      // 🔥 Fix: Ensure only one toolbar is present
      setTimeout(() => {
        document.querySelectorAll(".ql-toolbar").forEach((toolbar, index) => {
          if (index > 0) {
            toolbar.remove();
          }
        });
      }, 100);
    }
  }, []);

  return (
    <div className="relative">
      {/* ✅ No duplicate Quill instance */}
      <ReactQuill
        theme="snow"
        value={editorValue}
        onChange={(content) => {
          setEditorValue(content);
          onChange(content);
        }}
        modules={{
          toolbar: [
            [{ font: [] }],
            [{ header: [1, 2, 3, 4, 5, 6, false] }],
            [{ size: ["small", false, "large", "huge"] }],
            ["bold", "italic", "underline", "strike"],
            [{ color: [] }, { background: [] }],
            [{ script: "sub" }, { script: "super" }],
            [{ list: "ordered" }, { list: "bullet" }],
            [{ indent: "-1" }, { indent: "+1" }],
            [{ align: [] }],
            ["blockquote", "code-block"],
            ["link", "image", "video"],
            ["clean"],
          ],
        }}
        placeholder="Write a detailed product description here..."
        className="bg-transparent border border-gray-700 text-white rounded-md"
        style={{
          minHeight: "250px",
        }}
      />

      <style>
        {`
          .ql-toolbar {
            background: transparent;
            border-color: #444;
          }

          .ql-container {
            background: transparent !important;
            border-color: #444;
            color: white;
          }

          .ql-picker {
            color: white !important;
          }

          .ql-editor {
            min-height: 200px;
          }

          .ql-snow {
            border-color: #444 !important;
          }

          .ql-editor.ql-blank::before {
            color: #aaa !important;
          }

          .ql-picker-options {
            background: #333 !important;
            color: white !important;
          }

          .ql-picker-item {
            color: white !important;
          }

          .ql-stroke {
            stroke: white !important;
          }
        `}
      </style>
    </div>
  );
};

export default RichTextEditor;
