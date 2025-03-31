import { useEffect, useState } from "react";
import { Project } from "../types/Project";
import { useNavigate } from "react-router-dom";
import { fetchProjects } from "../api/projectsAPI";
import Pagination from "./pagination";

function ProjectList({ selectedCategories }: { selectedCategories: string[] }) {
  const [projects, setProjects] = useState<Project[]>([]);
  const [pageSize, setPageSize] = useState<number>(10);
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [totalPages, setTotalPages] = useState<number>(0);
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadProjects = async () => {
      try {
        setLoading(true);
        const data = await fetchProjects(
          pageSize,
          pageNumber,
          selectedCategories
        );
        setProjects(data.projects);
        setTotalPages(Math.ceil(data.totalNumProjects / pageSize));
      } catch (error) {
        setError((error as Error).message);
      } finally {
        setLoading(false);
      }
    };

    loadProjects();
  }, [pageSize, pageNumber, selectedCategories]);

  if (loading) return <p>Loadings Projects...</p>;
  if (error) return <p className="text-red-500">Error: {error}</p>;
  return (
    <>
      <h1>Water Project</h1>
      <br />
      {projects.map((p) => (
        <div id="projectCard" className="card">
          <h3 className="card-title">{p.projectName}</h3>
          <ul className="card-body">
            <li>
              <strong>Project Type:</strong> {p.projectType}
            </li>
            <li>
              <strong>Regional Program: </strong>
              {p.projectRegionalProgram}
            </li>
            <li>
              <strong>Impact: </strong>
              {p.projectImpact} Individuals Served
            </li>
            <li>
              <strong>Project Phase:</strong> {p.projectPhase}
            </li>
            <li>
              <strong>Project Status:</strong> {p.projectFunctionalityStatus}
            </li>
          </ul>

          <button
            className="btn btn-success"
            onClick={() => navigate(`/donate/${p.projectName}/${p.projectId}`)}
          >
            Donate
          </button>
        </div>
      ))}
      <Pagination
        currentPage={pageNumber}
        totalPages={totalPages}
        pageSize={pageSize}
        onPageChange={setPageNumber}
        onPageSizeChange={(newSize) => {
          setPageSize(newSize);
          setPageNumber(1);
        }}
      />
    </>
  );
}

export default ProjectList;
