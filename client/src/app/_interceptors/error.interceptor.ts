import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { NavigationExtras, Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((response: HttpErrorResponse) => {
        if (response) {
          switch (response.status) {
            case 400:
              if (response.error.errors) {
                const modelStateErrors = [];
                for (const key in response.error.errors) {
                  if (response.error.errors[key]) {
                    modelStateErrors.push(response.error.errors[key]);
                  }
                }
                throw modelStateErrors.flat();
              } else {
                this.toastr.error(response.error, response.status.toString());
              }
              break;
            case 401:
              this.toastr.error('Unauthorized', response.status.toString());
              break;
            case 404:
              this.router.navigateByUrl('not-found');
              break;
            case 500:
              const navigationErrors: NavigationExtras = {
                state: { error: response.error },
              };
              this.router.navigateByUrl('server-error', navigationErrors);
              break;
            default:
              this.toastr.error('Unexpected error');
              console.log(response);
          }
        }
        throw response;
      })
    );
  }
}
