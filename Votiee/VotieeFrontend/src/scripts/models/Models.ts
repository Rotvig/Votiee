module Models {

    export class SurveySessionViewModel {
        constructor() {
            this.Survey = new SurveyArchivedSessionViewModel();
        }

        SessionCode: string;
        CurrentSurveyItem: number;
        SurveyItemActive: boolean;
        VotingOpen: boolean;
        ShowResults: boolean;
        SurveyActive: boolean;
        Survey: SurveyArchivedSessionViewModel;
    }

    export class Answer {
        AnswerText: string;
        Order: number;
        AnswerId: Number;
        Last: boolean;
    }

    export class SurveyViewModel {
        constructor() {
            this.SurveyId = 0;
            this.SurveyItems = new Array<SurveyItemViewModel>();
            this.Name = "";
        }

        SurveyId: number;
        Name: string;
        SurveyItems: Array<SurveyItemViewModel>;
    }

    export class SurveyItemViewModel {
        constructor() {
            this.Answers = new Array<Answer>();
        }

        SurveyItemId: number;
        Order: number;
        QuestionText: string;
        Answers: Answer[];
        Last: boolean;
    }

    export class VotingViewModel {
        SurveyActive: boolean;
        SurveyItemActive: boolean;      
        Answered: boolean;
        VotingOpen: boolean;
        ShowResults: boolean;
        SelectedAnswer: number;
        SurveyItem: SurveyItemViewModel;
        AnswerResults: AnswerResultViewModel[];
    }

    export class PresenterViewModel {
        SurveyActive: boolean;
        SurveyItemActive: boolean;
        VotingOpen: boolean;
        ShowResults: boolean;
        SelectedAnswer: number;
        SurveyItem: SurveyItemViewModel;
        AnswerResults: AnswerResultViewModel[];
    }

    export interface AnswerResultViewModel {
        AnswerText: string;
        Order: number;
        VoteCount: number;
        Marked: boolean;
    }

    export interface ResultsViewModel {
        SurveyItemId: number;
        QuestionText?: string;
        AnswerResults?: AnswerResultViewModel[];
        LastSurveyItem: boolean;
    }

    export class SurveysOverviewViewModel {
        constructor() {
            this.Surveys = new Array<SurveysOverviewSurveyViewModel>();
        }

        Surveys: SurveysOverviewSurveyViewModel[];
    }

    export class SurveysOverviewSurveyViewModel {
        SurveyCode: string;
        Name: string;
    }

    export class StatisticOverviewSurveysViewModel {
        Name: string;
        Surveys: Array<StatisticsArchivedSurveysViewModel>;
    }

    export class StatisticsArchivedSurveysViewModel {
        SurveyId: number;
        ArchiveDate: string;
        Name: String;
    }

    export class SurveyStatisticsViewModel {
        constructor() {
            this.SurveyItems = new Array<SurveyItemStatisticsViewModel>();
        }

        Name: string;
        TemplateName: string;
        ArchiveDate: string;
        SurveyItems: SurveyItemStatisticsViewModel[];
    }

    export class SurveyItemStatisticsViewModel {
        constructor() {
            this.AnswerResults = new Array<AnswerResultViewModel>();
        }

        QuestionText: string;
        Order: number;
        AnswerResults: AnswerResultViewModel[];
    }

    export class SurveyItemSessionViewModel extends SurveyItemStatisticsViewModel{
        Active: boolean;
        ShowResults: boolean;
        VotingOpen: boolean;
        VoteCount: number;
    }

    export class SurveyArchivedSessionViewModel {
        constructor() {
            this.SurveyItems = new Array<SurveyItemSessionViewModel>();
        }

        Name: string;
        TemplateName: string;
        ArchiveDate: string;
        SurveyItems: SurveyItemSessionViewModel[];
    }


}