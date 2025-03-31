import { useParams } from "react-router-dom";

export default function ViewRequest() {
  const { id } = useParams<{ id: string }>();

  return <div>
    <h1>{id}</h1>
  </div>
}