import { useCallback, useEffect, useState } from "react";
import { useDropzone } from "react-dropzone";
import "./App.css";

type ImageState = { file: File; thumb: string };

function App() {
  const [image, setImage] = useState<ImageState>();
  const [result, setResult] = useState<string | undefined>();

  const onDrop = useCallback((acceptedFiles) => {
    const [image] = acceptedFiles;
    setResult(undefined);
    setImage({ file: image, thumb: URL.createObjectURL(image) });
  }, []);

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    accept: "image/*",
    multiple: false,
    onDrop,
  });

  const postImage = (formData: FormData) =>
    fetch("/api/happy", {
      method: "POST",
      body: formData,
    })
      .then((response) => response.text())
      .then((message) => {
        setResult(message);
      })
      .catch((error) => {
        console.error(error);
      });

  useEffect(() => {
    if (!!image) {
      const formData = new FormData();
      formData.append("image", image.file);
      postImage(formData);
    }

    return () => {
      if (!!image) {
        URL.revokeObjectURL(image.thumb);
      }
    };
  }, [image]);

  return (
    <div className="App">
      <h1>Happy?</h1>
      <div className="drop-zone" {...getRootProps()}>
        <input {...getInputProps()} />
        {isDragActive ? (
          <p>Drop picture here</p>
        ) : (
          <p>
            Drag and drop a picture of yourself here, to see if you are happy
          </p>
        )}
      </div>
      {!!image && <img src={image.thumb} alt="Preview" />}
      {!!result && <h1>{result}</h1>}
    </div>
  );
}

export default App;
