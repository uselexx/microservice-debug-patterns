import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { BehaviorSubject, Subscription, Observable } from 'rxjs';
import { ApiService } from '../../data-access/api/api.service';


export class MoviesService extends DataSource<any> {
  private cachedData = Array.from<any>({ length: 0 });
  private dataStream = new BehaviorSubject<any[]>(this.cachedData);
  private subscription = new Subscription();

  private lastCursor: number | null = null;

  // 1. Add this Subject to track loading state
  private loadingSubject = new BehaviorSubject<boolean>(false);
  
  // 2. Expose it as an Observable for the template
  public loading$ = this.loadingSubject.asObservable();

  constructor(private apiService: ApiService) {
    super();
    this._fetchPage(); // Initial load
  }

  connect(collectionViewer: CollectionViewer): Observable<any[]> {
    this.subscription.add(
      collectionViewer.viewChange.subscribe(range => {
        // If user scrolls near the end of our current cache, fetch more
        if (range.end > this.cachedData.length - 10 && !this.loadingSubject.value) {
          this._fetchPage();
        }
      })
    );
    return this.dataStream;
  }

  disconnect(): void {
    this.subscription.unsubscribe();
  }

  private _fetchPage() {
    if (this.loadingSubject.value) return;
    
    // 3. Set loading to true
    this.loadingSubject.next(true);

    // 1. Match your .NET Route: /movies/{cursor}/{pageSize}
    // If lastCursor is null (first load), we use 0
    const cursor = this.lastCursor ?? 0;
    const pageSize = 50;
    const url = `/movies/${cursor}/${pageSize}`;

    this.apiService.get<any>(url).subscribe({
      next: (res) => {
        // 2. Access the data through your Response Record hierarchy: 
        // res (GetMoviesResponse) -> movies (PagedResponse) -> data (List<GetMovieResponse>)
        const newItems = res.movies.data;

        this.cachedData = [...this.cachedData, ...newItems];
        this.dataStream.next(this.cachedData);

        // 3. Update cursor from res.movies.nextCursor
        this.lastCursor = res.movies.nextCursor;
        this.loadingSubject.next(false);
      },
      error: (err) => {
        console.error('Failed to fetch movies', err);
        this.loadingSubject.next(false);
      }
    });
  }
}