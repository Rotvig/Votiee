/// <reference path='./AnswerChoice.ts' />
/// <reference path='../../../controls/ResultsView.ts' />
var answerChoice = React.createFactory(VotingPage.AnswerChoice);
var ResultsView = React.createFactory(SharedComponents.ResultsView);
var AnswerResultViewComponent = React.createFactory(SharedComponents.AnswerResultView);
var pieChart = React.createFactory(SharedComponents.Piechart);

module VotingPage {


    //State and props interfaces
    export interface AnswerSelectorProps {
        answered: boolean;
        votingOpen: boolean;
        showResults: boolean;
        answers: Array<Models.Answer>;
        selectedAnswer: number;
        answerResults: Models.AnswerResultViewModel[];
        selectAnswer: Function;
    }

    export interface AnswerSelectorState {
        buttonsDisabled?: boolean;
    }

    export class AnswerSelector extends React.Component<AnswerSelectorProps, AnswerSelectorState> {
        state: AnswerSelectorState = {
            buttonsDisabled: false
        };

        render() {

            //Create list of answerChoice components based on Answers in list
            var answerChoiceList = [];

            this.props.answers.forEach(
                answer => answerChoiceList.push(
                    answerChoice({
                        answerNumber: answer.Order,
                        answerText: answer.AnswerText,
                        disabled: (this.props.answered ||
                            !this.props.votingOpen),
                        selectedAnswer: this.props.selectedAnswer,
                        selectAnswer: this.props.selectAnswer
                    })));

            //Create state comment for voting done
            const votingDone = div({ className: "selected-view" },
                h3({ className: "voting-done-text" }, "Thank you for voting!"),
                span({ className: "glyphicon glyphicon-ok icon-ok" }));

            //Create state comment for voting closed
            const votingClosed = div({ className: "voting-closed-view" },
                p({ className: "voting-closed-text" }, "You can't vote right now."));

            //Create resultsView for showResults
            const answerResults = div({ className: "show-results-view" },
                div({ className: "results-info" },
                    h3({ className: "results-text" }, "Voting Results"),
                    (!this.props.votingOpen) ? span({ className: "glyphicon glyphicon-stats icon-stats" }) : null),
                ResultsView({ answerResults: this.props.answerResults }));

            //Set state comment based on state
            const selectorView = div({ className: "selector-view" }, answerChoiceList);
            let viewContent = selectorView;
            if (!this.props.showResults) {
                if (this.props.answered) {
                    //Show voting done
                    viewContent = div({},
                        votingDone,
                        selectorView);
                } else if (!this.props.votingOpen) {
                    //Show voting closed
                    viewContent = div({},
                        votingClosed,
                        selectorView);
                }
            } else {
                //Show results
                if (!this.props.votingOpen || this.props.answered) {
                    //Show results only when voting is closed
                    viewContent = answerResults;
                } else {
                    //Show answerChoices and results when voting is open
                    viewContent = div({},
                        selectorView,
                        answerResults);
                }
            }

            //Return view with stateComment
            return div({ className: "answer-selector" }, viewContent);
        }
    }
} 