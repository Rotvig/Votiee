module VotingPage {

    //Props and state interfaces
    export class AnswerChoiceProps {
        answerNumber: number;
        answerText: string;
        disabled: boolean;
        selectedAnswer: number;
        selectAnswer: Function;
    }


    export class AnswerChoice extends React.Component<AnswerChoiceProps, {}>
    {
        render() {
            //Show button with answer
            return div({ className: 'answer-choice' },
                        button({
                            className: "btn answer-button" + ((this.props.selectedAnswer !== this.props.answerNumber) ?
                                " btn-default" : " btn-success"),
                            disabled: this.props.disabled,
                            onClick:  () => { this.props.selectAnswer(this.props.answerNumber); }
                        }, h5({},this.props.answerText))
                    );


        }

    }

} 