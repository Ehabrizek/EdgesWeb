import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { saveAs } from 'file-saver';
import { tap } from 'rxjs';


@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    _event;
    file;

    constructor(private http: HttpClient) {}

    ngOnInit(): void {}

    process() {
        if (this._event) {
            var test = this.file;
            var inputFile: File = this._event.target.files[0];

            if (inputFile) {
                this.sendPostRequest(inputFile);
            }
            else {
                alert('No files were input!')
            }
        }
        else {
            alert('No files were input!')
        }
    }

    sendPostRequest(inputFile: File) {
        var sheet = (<HTMLInputElement>document.getElementById('sheet')).value;
        var protein = (<HTMLInputElement>document.getElementById('protein')).value
        var pathwaydesc = (<HTMLInputElement>document.getElementById('pathwaydesc')).value;
        var pathwayid = (<HTMLInputElement>document.getElementById('pathwayid')).value;

        var formData = new FormData();

        formData.append("fileData", inputFile);
        formData.append("sheet", sheet);
        formData.append("protein", protein);
        formData.append("pathwaydesc", pathwaydesc);
        formData.append("pathwayid", pathwayid);
        this.getTextFile(formData).subscribe();
    }

    getTextFile(formData: FormData) {
        return this.http.post('/Edges', formData, {responseType: 'text'})
          .pipe(
            tap({
              next: (data) => {this.logSuccess(data)},
              error: (error) => this.logError(error)
            })
          );
      }

    logSuccess(data) {
        this.file = data;
        var resultDisplay = document.getElementById('resultDisplay');
        if (resultDisplay != undefined) {
            resultDisplay.innerHTML = 'Success!'
        }
    }

    logError(error) {
        var resultDisplay = document.getElementById('resultDisplay');
        if (resultDisplay != undefined) {
            resultDisplay.innerHTML = error.error;
        }
    }

    fileValidation(event) {
        this._event = event;
        const inputFile: File = this._event.target.files[0];
        if (inputFile) {
            var allowedExtensions = /(\.xls|\.xlsx)$/i;
            if (!allowedExtensions.exec(inputFile.name)) {
                alert('File must having extension .xls or .xlsx');
                event.target.value = null;
                return false;
            }
            else {
                return true;
            }
        }
        event.target.value = null;
        return false;
    }

    download() {
        if (this.file != null) {
            var data = new Blob([this.file], { type: 'text/plain' });
            saveAs(data, 'EdgesOutput.csv');
        }
        else {
            alert('No files to download! Try processing first, or retry with valid input!');
        }
    }
}
