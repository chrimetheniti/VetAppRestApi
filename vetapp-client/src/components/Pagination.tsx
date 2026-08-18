import {Button} from "@/components/ui/button.tsx";
import {ChevronLeft, ChevronRight} from "lucide-react";

interface PaginationProps {
  pageNumber: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

const Pagination = ({pageNumber, totalPages, onPageChange}: PaginationProps) => {
  // Nothing to paginate — hide the controls entirely.
  if (totalPages <= 1) return null;

  const canGoPrev = pageNumber > 1;
  const canGoNext = pageNumber < totalPages;

  return (
    <div className="flex items-center justify-center gap-4 mt-6">
      <Button
        variant="outline"
        size="sm"
        onClick={() => onPageChange(pageNumber - 1)}
        disabled={!canGoPrev}
        aria-label="Previous page"
      >
        <ChevronLeft className="w-4 h-4 mr-1"/>
        Previous
      </Button>

      <span className="text-sm text-muted-foreground">
        Page <span className="font-medium text-foreground">{pageNumber}</span> of{" "}
        <span className="font-medium text-foreground">{totalPages}</span>
      </span>

      <Button
        variant="outline"
        size="sm"
        onClick={() => onPageChange(pageNumber + 1)}
        disabled={!canGoNext}
        aria-label="Next page"
      >
        Next
        <ChevronRight className="w-4 h-4 ml-1"/>
      </Button>
    </div>
  );
};

export default Pagination;
