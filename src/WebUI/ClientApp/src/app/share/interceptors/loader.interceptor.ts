import { Injectable } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize, tap } from 'rxjs/operators';
import { SpinnerService } from 'src/app/share/component/spinner/spinner.service';
@Injectable()
export class LoaderInterceptor implements HttpInterceptor {
  constructor (public loaderService: SpinnerService) { }
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!(req.url.includes('../assets') && req.headers.has('Skip-Loading'))) {
      this.loaderService.requestStarted();
      return this.handler(next, req);
    } else {
      return next.handle(req);
    }
  }

  handler(next, request) {
    return next.handle(request).pipe(
      tap(
        (event) => {
          if (event instanceof HttpResponse) {
            this.loaderService.requestEnded();
          }
        },
        (error: HttpErrorResponse) => {
          this.loaderService.resetSpinner();
          throw error;
        }
      ),
    );
  }
}
