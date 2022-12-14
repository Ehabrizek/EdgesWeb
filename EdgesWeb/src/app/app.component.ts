import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { saveAs } from 'file-saver';
import { tap } from 'rxjs';


@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    inputFile;
    outputFile;

    constructor(private http: HttpClient) {}

    ngOnInit(): void {}

    fileValidation(event) {
        this.inputFile = event.target.files[0];
        if (this.inputFile) {
            var allowedExtensions = /(\.xls|\.xlsx)$/i;
            if (!allowedExtensions.exec(this.inputFile.name)) {
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

    process() {
        if (this.inputFile) {
            var sheet = (<HTMLInputElement>document.getElementById('sheet')).value;
            var protein = (<HTMLInputElement>document.getElementById('protein')).value
            var pathwaydesc = (<HTMLInputElement>document.getElementById('pathwaydesc')).value;
            var pathwayid = (<HTMLInputElement>document.getElementById('pathwayid')).value;

            var formData = new FormData();
            formData.append("fileData", this.inputFile);
            formData.append("sheet", sheet);
            formData.append("protein", protein);
            formData.append("pathwaydesc", pathwaydesc);
            formData.append("pathwayid", pathwayid);

            this.sendPostRequest(formData).subscribe();
        }
        else {
            alert('No files were input!')
        }
    }

    sendPostRequest(formData :FormData) {
        return this.http.post('/Edges', formData, {responseType: 'text'})
          .pipe(
            tap({
              next: (data) => this.logSuccess(data),
              error: (error) => this.logError(error)
            })
          );
    }

    logSuccess(data) {
        this.outputFile = data;
        this.updateResultDisplay('Success!');
    }

    logError(error) {
        this.updateResultDisplay(error.error);
    }

    updateResultDisplay(message :string) {
        var resultDisplay = document.getElementById('resultDisplay');
        if (resultDisplay) {
            resultDisplay.innerHTML = message;
        }
    }

    download() {
        if (this.outputFile != null) {
            var data = new Blob([this.outputFile], { type: 'text/plain' });
            saveAs(data, 'EdgesOutput.csv');
        }
        else {
            alert('No files to download! Try processing first, or retry with valid input!');
        }
    }
}
